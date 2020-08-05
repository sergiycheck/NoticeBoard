using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;

namespace NoticeBoard.Repositories
{
    public class NotificationsRepository : GenericRepository<Notification>, INotificationsRepository
    {
        public NotificationsRepository(NoticeBoardDbContext context)
            : base(context)
        {
               
        }
        public bool NotificationsExists(int id)
        {
            return _dbContext.Notifications.Any(e => e.NotificationId == id);
        }
    }
}
