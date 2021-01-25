using AutoMapper;
using NoticeBoard.Models;
using NoticeBoard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NoticeBoard.Models.ViewModels.RoleControllerViewModels;

namespace NoticeBoard.Helpers.CustomMapper
{
    public interface ICustomMapper 
    {
        EditUserViewModel Map_EditUserViewModel(CustomUser user);
        CustomUser Map_EditUserViewModel(EditUserViewModel user);
        IEnumerable<CustomRoleViewModel> Map_CollectionCustomRoleViewModel(IEnumerable<CustomRole> customRoleList);
        IEnumerable<CustomUserForRoleActionsViewModel> Map_CollectionCustomUserForRoleActionsViewModel(IEnumerable<CustomUser> customUserCollection);
    }
    public class CustomMapper: ICustomMapper
    {
        public T1 MapFromOneToAnother<T1, T2>(T2 entity) 
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<T2, T1>());
            var mapper = new Mapper(config);
            T1 entityMapped = mapper.Map<T1>(entity);
            return entityMapped;
        }
        public EditUserViewModel Map_EditUserViewModel(CustomUser user) 
        {  
            var editUserViewModel = MapFromOneToAnother<EditUserViewModel,CustomUser>(user);
            return editUserViewModel;
        }
        public CustomUser Map_EditUserViewModel(EditUserViewModel editUserViewModel) 
        {
            var customUser = MapFromOneToAnother<CustomUser,EditUserViewModel>(editUserViewModel);
            return customUser;
        }
        public IEnumerable<CustomRoleViewModel> Map_CollectionCustomRoleViewModel(IEnumerable<CustomRole> customRoleList) 
        {
            var collectionOfCustomRoleViewModels = new List<CustomRoleViewModel>();
            customRoleList.ToList().ForEach(cr => collectionOfCustomRoleViewModels.Add(Map_CustomRoleViewModel(cr)));
            return collectionOfCustomRoleViewModels;
        }
        public CustomRoleViewModel Map_CustomRoleViewModel(CustomRole customRole) 
        {
            var customRoleViewModel = new CustomRoleViewModel() { Id = customRole.Id, Name = customRole.Name };
            return customRoleViewModel;
        }


        public IEnumerable<CustomUserForRoleActionsViewModel> Map_CollectionCustomUserForRoleActionsViewModel(IEnumerable<CustomUser> customUserCollection)
        {
            var collectionOfUserForRoleActionsViewModels = new List<CustomUserForRoleActionsViewModel>();
            customUserCollection.ToList().ForEach(cu => collectionOfUserForRoleActionsViewModels.Add(Map_CustomUserForRoleActionsViewModel(cu)));
            return collectionOfUserForRoleActionsViewModels;
        }


        public CustomUserForRoleActionsViewModel Map_CustomUserForRoleActionsViewModel(CustomUser customUser)
        {
            var customUserForRoleActionsViewModel = new CustomUserForRoleActionsViewModel() { Id = customUser.Id, Email = customUser.Email };
            return customUserForRoleActionsViewModel;
        }
    }
}
