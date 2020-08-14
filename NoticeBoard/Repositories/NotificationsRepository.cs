using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;
using NoticeBoard.Models.ViewModels;

namespace NoticeBoard.Repositories
{
    public class NotificationsRepository : GenericRepository<Notification>, INotificationsRepository
    {
        public NotificationsRepository(NoticeBoardDbContext context)
            : base(context)
        {
               
        }
        public async Task<Notification> NotificationIncludeComments(int?id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(n=>n.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        
    }
}
