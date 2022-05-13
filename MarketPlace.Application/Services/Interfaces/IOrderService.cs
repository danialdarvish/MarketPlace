using System;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.DataLayer.Entities.ProductOrder;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IOrderService : IAsyncDisposable
    {
        #region Order

        Task<long> AddOrderForUser(long userId);
        Task<Order> GetUserLatestOpenOrder(long userId);
        Task<int> GetTotalOrderPriceForPayment(long userId);
        Task PayOrderProductPriceToSeller(long userId, long refId);

        #endregion

        #region Order detail

        Task AddProductToOpenOrder(AddProductToOrderDto order, long userId);
        Task<UserOpenOrderDto> GetUserOpenOrderDetail(long userId);
        Task<bool> RemoveOrderDetail(long detailId, long userId);
        Task ChangeOrderDetailCount(long detailId, long userId, int count);

        #endregion
    }
}
