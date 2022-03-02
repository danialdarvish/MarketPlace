using MarketPlace.DataLayer.Context;
using MarketPlace.DataLayer.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.DataLayer.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly MarketPlaceDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(MarketPlaceDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task AddEntity(TEntity entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.LastUpdateDate = entity.CreateDate;
            await _dbSet.AddAsync(entity);
        }

        public void DeleteEntity(TEntity entity)
        {
            entity.IsDelete = true;
            EditEntity(entity);
        }

        public async Task DeleteEntity(long id)
        {
            TEntity entity = await GetEntityById(id);
            if (entity != null) DeleteEntity(entity);
        }

        public void DeletePermanent(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task DeletePermanent(long id)
        {
            TEntity entity = await GetEntityById(id);
            if (entity != null) DeletePermanent(entity);
        }

        public async ValueTask DisposeAsync()
        {
            if (_context != null)
                await _context.DisposeAsync();
        }

        public void EditEntity(TEntity entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            _dbSet.Update(entity);
        }

        public async Task<TEntity> GetEntityById(long id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<TEntity> GetQuery()
        {
            return _dbSet.AsQueryable();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
