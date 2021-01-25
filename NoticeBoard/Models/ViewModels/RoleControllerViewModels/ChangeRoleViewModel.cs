using Microsoft.AspNetCore.Identity;
using NoticeBoard.AuthorizationsManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoticeBoard.Models.ViewModels.RoleControllerViewModels;

namespace NoticeBoard.Models.ViewModels
{
    public class ChangeRoleViewModel:BaseViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public ICollection<string> AllRoles { get; set; }
        public ICollection<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<string>();
            UserRoles = new List<string>();
        }
    }
}
