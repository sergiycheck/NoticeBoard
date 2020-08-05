using NoticeBoard.Interfaces;
using NoticeBoard.Models;

namespace NoticeBoard.Interfaces
{
    public interface INotificationsRepository:IGenericRepository<Notification>
    {
        bool NotificationsExists(int id);
    }
}