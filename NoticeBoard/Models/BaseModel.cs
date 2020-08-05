namespace NoticeBoard.Models
{
    public class BaseModel
    {
        public int Id{get;set;}
        public string OwnerID { get; set; }//for asp net identity
    }
}