using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticeBoard.Models
{
    public class Comment
    {
        public int CommentId{get;set;}
        public int NotificationId{get;set;}
        public Notification Notification{get;set;}
        public string OwnerID { get; set; }//for asp net identity
        public string Description{get;set;}

    }
}