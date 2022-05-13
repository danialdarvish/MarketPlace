using System;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.DataLayer.Entities.ProductOrder;
using MarketPlace.DataLayer.Entities.Products;
using MarketPlace.DataLayer.Entities.Wallet;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        #region Constructor

        private readonly ISellerWalletService _sellerWalletService;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<ProductDiscount> _productDiscountRepository;
        private readonly IGenericRepository<ProductDiscountUse> _productDiscountUseRepository;

        public OrderService(ISellerWalletService sellerWalletService, IGenericRepository<Order> orderRepository, IGenericRepository<OrderDetail> orderDetailRepository, IGenericRepository<ProductDiscount> productDiscountRepository, IGenericRepository<ProductDiscountUse> productDiscountUseRepository)
        {
            _orderRepository = orderRepository;
            _sellerWalletService = sellerWalletService;
            _orderDetailRepository = orderDetailRepository;
            _productDiscountRepository = productDiscountRepository;
            _productDiscountUseRepository = productDiscountUseRepository;
        }

        #endregion

        #region Order

        public async Task<long> AddOrderForUser(long userId)
        {
            var order = new Order { UserId = userId };

            await _orderRepository.AddEntity(order);
            await _orderRepository.SaveChanges();

            return order.Id;
        }

        public async Task<Order> GetUserLatestOpenOrder(long userId)
        {
            if (!await _orderRepository.GetQuery().AnyAsync(x => x.UserId == userId && !x.IsPaid))
                await AddOrderForUser(userId);

            var userOpenOrder = await _orderRepository.GetQuery()
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.ProductColor)
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductDiscounts)
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsPaid);

            return userOpenOrder;
        }

        public async Task<int> GetTotalOrderPriceForPayment(long userId)
        {
            var userOpenOrder = await GetUserLatestOpenOrder(userId);
            int totalPrice = 0;
            int discount = 0;
            foreach (var detail in userOpenOrder.OrderDetails)
            {
                var oneProductPrice = detail.ProductColor != null
                    ? detail.Product.Price + detail.ProductColor.Price
                    : detail.Product.Price;
                var productDiscount = await _productDiscountRepository.GetQuery()
                    .Include(x => x.ProductDiscountUse)
                    .OrderByDescending(x => x.CreateDate)
                    .FirstOrDefaultAsync(x => x.ProductId == detail.ProductId &&
                                              x.DiscountNumber - x.ProductDiscountUse.Count > 0);

                if (productDiscount != null)
                {
                    discount = (int)Math.Ceiling(oneProductPrice * productDiscount.Percentage / (decimal)100);
                }

                totalPrice += detail.Count * (oneProductPrice - discount);
            }

            return totalPrice;
        }

        public async Task PayOrderProductPriceToSeller(long userId, long refId)
        {
            var openOrder = await GetUserLatestOpenOrder(userId);
            foreach (var detail in openOrder.OrderDetails)
            {
                var productPrice = detail.Product.Price;
                var productColorPrice = detail.ProductColor?.Price ?? 0;
                var discount = 0;
                var totalPrice = detail.Count * (productPrice + productColorPrice);
                var productDiscount = await _productDiscountRepository.GetQuery()
                    .Include(x => x.ProductDiscountUse)
                    .OrderByDescending(x => x.CreateDate)
                    .FirstOrDefaultAsync(x => x.ProductId == detail.ProductId &&
                                              x.DiscountNumber - x.ProductDiscountUse.Count > 0);

                if (productDiscount != null)
                {
                    discount = (int)Math.Ceiling(totalPrice * productDiscount.Percentage / (decimal)100);
                    var newDiscountUse = new ProductDiscountUse
                    {
                        UserId = userId,
                        ProductDiscountId = productDiscount.Id
                    };

                    await _productDiscountUseRepository.AddEntity(newDiscountUse);
                }

                var totalPriceWithDiscount = totalPrice - discount;

                await _sellerWalletService.AddWallet(new SellerWallet
                {
                    SellerId = detail.Product.SellerId,
                    Price = (int)Math.Ceiling(totalPriceWithDiscount * (100 - detail.Product.SiteProfit) / (double)100),
                    TransactionType = TransactionType.Deposit,
                    Description = $"پرداخت مبلغ {totalPriceWithDiscount} تومان جهت فروش {detail.Product.Title} به تعداد {detail.Count} عدد با سهم {100 - detail.Product.SiteProfit} درصد"
                });

                detail.ProductPrice = totalPriceWithDiscount;
                detail.ProductColorPrice = productColorPrice;
                _orderDetailRepository.EditEntity(detail);
            }

            openOrder.IsPaid = true;
            openOrder.TracingCode = refId.ToString();
            openOrder.PaymentDate = DateTime.Now;

            _orderRepository.EditEntity(openOrder);
            await _orderRepository.SaveChanges();
        }

        #endregion

        #region Order detail

        public async Task AddProductToOpenOrder(AddProductToOrderDto order, long userId)
        {
            var openOrder = await GetUserLatestOpenOrder(userId);
            var similarOrder = openOrder.OrderDetails.FirstOrDefault(x =>
                x.ProductId == order.ProductId && x.ProductColorId == order.ProductColorId && !x.IsDelete);

            if (similarOrder == null)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = openOrder.Id,
                    ProductId = order.ProductId,
                    ProductColorId = order.ProductColorId,
                    Count = order.Count
                };

                await _orderDetailRepository.AddEntity(orderDetail);
                await _orderDetailRepository.SaveChanges();
            }
            else
            {
                similarOrder.Count += order.Count;
                await _orderDetailRepository.SaveChanges();
            }
        }

        public async Task<UserOpenOrderDto> GetUserOpenOrderDetail(long userId)
        {
            var userOpenOrder = await GetUserLatestOpenOrder(userId);

            return new UserOpenOrderDto
            {
                UserId = userId,
                Description = userOpenOrder.Description,
                Details = userOpenOrder.OrderDetails
                    .Where(x => !x.IsDelete)
                    .Select(x => new UserOpenOrderDetailItemDto
                    {
                        DetailId = x.Id,
                        Count = x.Count,
                        ColorName = x.ProductColor?.ColorName,
                        ProductColorId = x.ProductColorId,
                        ProductColorPrice = x.ProductColor?.Price ?? 0,
                        ProductId = x.ProductId,
                        ProductPrice = x.Product.Price,
                        ProductTitle = x.Product.Title,
                        ProductImageName = x.Product.ImageName,
                        DiscountPercentage = x.Product.ProductDiscounts
                        .OrderByDescending(s => s.CreateDate)
                        .FirstOrDefault(s => s.ExpireDate > DateTime.Now)?.Percentage
                    }).ToList()
            };
        }

        public async Task<bool> RemoveOrderDetail(long detailId, long userId)
        {
            var openOrder = await GetUserLatestOpenOrder(userId);

            var orderDetail = openOrder.OrderDetails.FirstOrDefault(x => x.Id == detailId);
            if (orderDetail == null) return false;

            _orderDetailRepository.DeleteEntity(orderDetail);
            await _orderDetailRepository.SaveChanges();

            return true;
        }

        public async Task ChangeOrderDetailCount(long detailId, long userId, int count)
        {
            var userOpenOrder = await GetUserLatestOpenOrder(userId);
            var detail = userOpenOrder.OrderDetails.FirstOrDefault(x => x.Id == detailId);
            if (detail != null)
            {
                if (count > 0) detail.Count = count;
                else _orderDetailRepository.DeleteEntity(detail);

                await _orderDetailRepository.SaveChanges();
            }
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _orderRepository.DisposeAsync();
            await _orderDetailRepository.DisposeAsync();
        }

        #endregion
    }
}