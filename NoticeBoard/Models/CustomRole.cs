using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Models
{
    public class CustomRole : IdentityRole
    {
        public CustomRole() { }
        public CustomRole(string name) : base(name) { }

        //public virtual TKey Id { get; set; }
        //public virtual string Name { get; set; }
        //public virtual string NormalizedName { get; set; }
        //public virtual string ConcurrencyStamp { get; set; }
    }
}
