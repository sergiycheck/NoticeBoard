using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Models;

namespace NoticeBoard.Data
{
    public class NoticeBoardDbContext : IdentityDbContext<CustomUser>
    {
        public DbSet<Notification> Notifications{get;set;}
        public DbSet<Comment>Comments{get;set;}
        public NoticeBoardDbContext(DbContextOptions<NoticeBoardDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
