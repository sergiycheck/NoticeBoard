using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity:class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(int? id);
        Task Create(TEntity entity);
        void Update(TEntity entity);
        Task Delete(int? id);
        Task SaveChangesAsync();
    }
}
