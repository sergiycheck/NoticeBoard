using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoticeBoard.Authorization;
using NoticeBoard.AuthorizationsManagers;
using NoticeBoard.Helpers.CustomMapper;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using NoticeBoard.Models.ViewModels;
using NoticeBoard.Models.ViewModels.RoleControllerViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Areas.CustomAuthorization.Controllers
{
    [Area("CustomAuthorization")]
    [Authorize(Roles = NotificationConstants.ContactAdministratorsRole)]
    public class CustomRoleController : Controller
    {
        private readonly ICustomRoleManager _customRoleManager;
        private readonly ICustomUserManager _customUserManager;
        private readonly ICustomMapper _customMapper;

        public CustomRoleController(
            ICustomRoleManager customRoleManager,
            ICustomUserManager customUserManager,
            ICustomMapper customMapper
            ) 
        {
            _customRoleManager = customRoleManager;
            _customUserManager = customUserManager;
            _customMapper = customMapper;
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            var customRoleViewModels = _customMapper
               .Map_CollectionCustomRoleViewModel(
                       await _customRoleManager.GetCustomRolesAsNoTracking());

            return View(new IndexViewModel() 
            {
                customCustomRoleViewModels = customRoleViewModels.ToList(),
                deleteRoleViewModel = new DeleteRoleViewModel() { }
            });
        }
           

        [HttpGet]
        public IActionResult Create() => View(new CreateRoleViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel roleViewModel)
        {
            if (!string.IsNullOrEmpty(roleViewModel.RoleName))
            {
                IdentityResult result = await _customRoleManager.CreateAsync(new CustomRole(roleViewModel.RoleName));
                if (result.Succeeded)
                {
                    return View(new CreateRoleViewModel() { ResponceMessage = "Role created successfully" });
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View(roleViewModel);
        }
        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(DeleteRoleViewModel deleteRoleViewModel) 
        {
            if (ModelState.IsValid) 
            {
                var role = await _customRoleManager.FindByIdAsync(deleteRoleViewModel.RoleId);
                if (role != null)
                {
                    var result = await _customRoleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        var customRoleViewModels = _customMapper
                               .Map_CollectionCustomRoleViewModel(
                                       await _customRoleManager.GetCustomRolesAsNoTracking());

                        return View("Index", new IndexViewModel()
                        {
                            customCustomRoleViewModels = customRoleViewModels.ToList(),
                            deleteRoleViewModel = new DeleteRoleViewModel() { ResponceMessage = $"role {role.Name} deleted successfully" }
                        });

                        
                    }
                }
                return View("Index");
            }
            return BadRequest("Entered values are not correct");
            
        }

        [HttpGet,ActionName("ListUsers")]
        public async Task<IActionResult> ListUsersAsync() 
        {
            return View(_customMapper
            .Map_CollectionCustomUserForRoleActionsViewModel(
            await _customUserManager.ListUsersAsync()));

        }



        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string userId) 
        {
            var user = await _customUserManager.FindByIdAsync(userId);
            if (user != null) 
            {
                var userRoles = await _customRoleManager.GetUserRolesAsNoTracking(user);
                var userRolesNames = new List<string>();
                if (userRoles != null)
                    userRoles.ToList().ForEach(ur => userRolesNames.Add(ur.Name));

                var allRoles = await _customRoleManager.GetCustomRolesAsNoTracking();
                var allRolesNames = new List<string>();
                if (allRoles != null)
                    allRoles.ToList().ForEach(r => allRolesNames.Add(r.Name));

                var changeRoleViewModel = new ChangeRoleViewModel()
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRolesNames,
                    AllRoles = allRolesNames
                };
                return View(changeRoleViewModel);
            }
            return View("Index");
        }

        [HttpPost, ActionName("EditUserRoles")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRolesConfirmed(ChangeRoleViewModel changeRoleViewModel) 
        {
            if (ModelState.IsValid) 
            {
                var user = await _customUserManager.FindByEmailAsync(changeRoleViewModel.UserEmail);
                if (user != null) 
                {
                    
                    var userRoles = await _customRoleManager.GetUserRolesAsNoTracking(user);
                    var userRolesNames = new List<string>();
                    if (userRoles != null)
                        userRoles.ToList().ForEach(ur => userRolesNames.Add(ur.Name));


                    if (changeRoleViewModel.UserRoles != null) 
                    {
                        var addedRoles = changeRoleViewModel.UserRoles.Except(userRolesNames);
                        var addRolesResult =  await _customUserManager.AddToRolesAsync(user, addedRoles);
                        if (!addRolesResult.Succeeded) 
                        {
                            return BadRequest("Cannot add user to selected roles");
                        }

                        var removedRoles = userRolesNames.Except(changeRoleViewModel.UserRoles);
                        var removeRolesResult = await _customUserManager.RemoveFromRolesAsync(user, removedRoles);

                        if (!removeRolesResult.Succeeded)
                        {
                            return BadRequest("Cannot remove user from selected roles");
                        }


                    }
                    changeRoleViewModel.ResponceMessage = $"User ( {changeRoleViewModel.UserEmail} ) roles edited successfully";
                    return View("EditUserRoles", changeRoleViewModel);
                }
            }
            return BadRequest("Entered values are not correct");
        }






    }
}
