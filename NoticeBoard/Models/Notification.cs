using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticeBoard.Models
{
    public class Notification:BaseModel
    {
        public int NotificationId{get;set;}
        public string Name{get;set;}

        public string Description{get;set;}
        public ICollection<Comment>Comments{get;set;}

    }
}
//dotnet aspnet-codegenerator controller -name NotificationController -m Notification -dc NoticeBoardDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries