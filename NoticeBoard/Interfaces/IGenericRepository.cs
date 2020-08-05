using System.Linq;
using System.Threading.Tasks;
using NoticeBoard.Models;

namespace NoticeBoard.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity:BaseModel
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(int? id);
        Task Create(TEntity entity);
        void Update(TEntity entity);
        Task Delete(int? id);
        Task SaveChangesAsync();
        bool EntityExists(int id);
    }
}
