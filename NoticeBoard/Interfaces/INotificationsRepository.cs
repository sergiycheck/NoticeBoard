using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using System.Threading.Tasks;

namespace NoticeBoard.Interfaces
{
    public interface INotificationsRepository:IGenericRepository<Notification>
    {
        Task<Notification> NotificationIncludeComments(int?id);
    }
}