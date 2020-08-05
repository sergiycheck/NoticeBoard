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
    public class CommentsRepository : GenericRepository<Comment>, ICommentsRepository
    {
        public CommentsRepository(NoticeBoardDbContext context)
            : base(context)
        {
               
        }
        public  IQueryable<Comment> CommentsIncludeNotification()
        {
            return _dbSet
                .AsNoTracking()
                .Include(n=>n.Notification);
        }
        public async Task<Comment> CommentIncludeNotification(int?id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Notification)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
