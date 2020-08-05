using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticeBoard.Models
{
    public class Comment:BaseModel
    {
        public int NotificationId{get;set;}
        public Notification Notification{get;set;}
        public string Description{get;set;}

    }
}