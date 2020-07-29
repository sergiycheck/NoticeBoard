using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NoticeBoard.Data
{
    public class NoticeBoardDbContext : IdentityDbContext
    {
        public NoticeBoardDbContext(DbContextOptions<NoticeBoardDbContext> options)
            : base(options)
        {
        }
    }
}
