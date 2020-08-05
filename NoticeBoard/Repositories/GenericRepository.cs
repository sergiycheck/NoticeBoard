using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Models;
using NoticeBoard.Interfaces;
using NoticeBoard.Data;

namespace NoticeBoard.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly NoticeBoardDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        
        public GenericRepository(NoticeBoardDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet=_dbContext.Set<TEntity>();
        }
        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<TEntity> GetById(int? id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public async Task Delete(int? id)
        {
            _dbSet.Remove(await GetById(id));
        }
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}