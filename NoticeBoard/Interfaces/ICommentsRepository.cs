using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Interfaces
{
    public interface ICommentsRepository:IGenericRepository<Comment>
    {
        IQueryable<Comment> CommentsIncludeNotification();
        Task<Comment> CommentIncludeNotification(int?id);
    }
}