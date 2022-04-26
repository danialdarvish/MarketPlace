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

        #endregion

        #region Order detail

        Task AddProductToOpenOrder(AddProductToOrderDto order, long userId);
        Task<UserOpenOrderDto> GetUserOpenOrderDetail(long userId);

        #endregion
    }
}
