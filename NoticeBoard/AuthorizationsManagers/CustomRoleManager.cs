using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NoticeBoard.Data;
using NoticeBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NoticeBoard.AuthorizationsManagers
{

    public interface ICustomRolesRespository 
    {
        Task<ICollection<CustomRole>> GetCustomRolesAsNoTracking();
        Task<bool> EntityExistsAsNoTracking(string id);
        Task<ICollection<CustomRole>> GetUserRolesAsNoTracking(CustomUser user);
    }
    public class CustomRolesRepository : ICustomRolesRespository
    {
        protected readonly NoticeBoardDbContext _dbContext;

        public CustomRolesRepository(NoticeBoardDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<bool> EntityExistsAsNoTracking(string id)
        {
            return await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Id == id);
        }

        public async Task<ICollection<CustomRole>> GetCustomRolesAsNoTracking()
        {
            return await _dbContext.Roles.AsNoTracking().ToListAsync();
        }
        public async Task<ICollection<CustomRole>> GetUserRolesAsNoTracking(CustomUser user)
        {
            var result = await _dbContext.UserRoles.AsNoTracking()
                .Where(ur => ur.UserId == user.Id)
                .Join(_dbContext.Roles.AsNoTracking(),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => new CustomRole() 
                { 
                    Name = r.Name,
                    ConcurrencyStamp = r.ConcurrencyStamp, 
                    Id = r.Id, 
                    NormalizedName = r.NormalizedName 
                }).ToListAsync();
            return result;
        }
    }

    public interface ICustomRoleManager 
    {
        Task<IdentityResult> CreateAsync(CustomRole role);
        Task<CustomRole> FindByIdAsync(string id);
        Task<IdentityResult> DeleteAsync(CustomRole role);
        Task<ICollection<CustomRole>> GetCustomRolesAsNoTracking();
        Task<ICollection<CustomRole>> GetUserRolesAsNoTracking(CustomUser user);
        Task<bool> RoleExistsAsync(string role);
    }

    public class CustomRoleManager:RoleManager<CustomRole>,ICustomRoleManager
    {
        private readonly ICustomRolesRespository _customRolesRespository;
        public CustomRoleManager(
            IRoleStore<CustomRole> store,
            IEnumerable<IRoleValidator<CustomRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<CustomRole>> logger,
            ICustomRolesRespository customRolesRespository
            ) : base(
                store, 
                roleValidators, 
                keyNormalizer, 
                errors,
                logger) 
        {
            _customRolesRespository = customRolesRespository;

        }

        public override Task<bool> RoleExistsAsync(string role) 
        {
            return base.RoleExistsAsync(role);
        }

        public override Task<IdentityResult> CreateAsync(CustomRole role)
        {
            return base.CreateAsync(role);
        }
        public override Task<CustomRole> FindByIdAsync(string id)
        {
            return base.FindByIdAsync(id);
        }
        public override Task<IdentityResult> DeleteAsync(CustomRole role)
        {
            return base.DeleteAsync(role);
        }
        public async Task<ICollection<CustomRole>> GetCustomRolesAsNoTracking() 
        {
            return await _customRolesRespository.GetCustomRolesAsNoTracking();
        }
        public async Task<ICollection<CustomRole>> GetUserRolesAsNoTracking(CustomUser user)
        {
            return await _customRolesRespository.GetUserRolesAsNoTracking(user);
        }


    }
}
