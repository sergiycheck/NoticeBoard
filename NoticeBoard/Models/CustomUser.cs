using System;
using Microsoft.AspNetCore.Identity;


namespace NoticeBoard.Models
{
    public class CustomUser:IdentityUser
    {
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string Country{get;set;}
        public string City{get;set;}
        public string Address{get;set;}

        //Implemented properties
        //Id
        //PhoneNumber
        //Email
        //UserName
    }
    //todo add city list
    //add phone country list
}