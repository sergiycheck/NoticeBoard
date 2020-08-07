using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace NoticeBoard.Authorization
{
    public static class NotificatinOperations
    {
        public static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name= NotificationConstants.CreateOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name= NotificationConstants.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name= NotificationConstants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name= NotificationConstants.DeleteOperationName};
        //TODO:add more operations for other roles (manager,employee and so on)
    }

    public class NotificationConstants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ContactAdministratorsRole = 
                                                              "NotificationAdministrators";
        //TODO:add more constants
    }
}