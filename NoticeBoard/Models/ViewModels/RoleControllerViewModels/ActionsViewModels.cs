using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Models.ViewModels.RoleControllerViewModels
{

    public class BaseViewModel
    {
        public string ResponceMessage { get; set; }
    }

    public class IndexViewModel 
    {
        public List<CustomRoleViewModel> customCustomRoleViewModels { get; set; }
        public DeleteRoleViewModel deleteRoleViewModel { get; set; }
    }

    public class CustomRoleViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class CreateRoleViewModel : BaseViewModel
    {
        [Required]
        public string RoleName { get; set; }

    }
    public class DeleteRoleViewModel : BaseViewModel
    {
        [Required]
        public string RoleId { get; set; }

    }

    public class CustomUserForRoleActionsViewModel : BaseViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
