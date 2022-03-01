using MarketPlace.DataLayer.Entities.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.DataLayer.Repository
{
    public interface IGenericRepository<TEntity> : IAsyncDisposable where TEntity : BaseEntity
    {
        IQueryable<TEntity> GetQuery();
        Task AddEntity(TEntity entity);
        Task<TEntity> GetEntityById(long id);
        void EditEntity(TEntity entity);
        void DeleteEntity(TEntity entity);
        Task DeleteEntity(long id);
        void DeletePermanent(TEntity entity);
        Task DeletePermanent(long id);
        Task SaveChanges();
    }
}
