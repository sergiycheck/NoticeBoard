using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Models.ViewModels
{
    public class NotificationViewModel
    {
        public Notification Notification { get; set; }
        public string OwnerName { get; set; }
        public List<CommentViewModel> CommentViewModel { get; set; }
        //add reply ability
    }
    public class CommentViewModel 
    {
        public Comment Comment { get; set; }
        public string OwnerName { get; set; }
    }
}
