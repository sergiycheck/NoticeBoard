using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Repositories
{
    public interface ICustomUserRepository
    {
        Task<CustomUser> GetUserByIdAsNoTracking(string userId);
        Task<bool> EntityExists(string id);

    }
    public class CustomUserRepository : ICustomUserRepository
    {
        protected readonly NoticeBoardDbContext _dbContext;

        public CustomUserRepository(NoticeBoardDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<CustomUser> GetUserByIdAsNoTracking(string userId)
        {
            
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<bool> EntityExists(string id)
        {
            return await _dbContext.Users.AsNoTracking().AnyAsync(e => e.Id == id);
        }

    }
}
