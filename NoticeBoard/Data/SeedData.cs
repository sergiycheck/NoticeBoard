using NoticeBoard.Authorization;
using NoticeBoard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using NoticeBoard.Interfaces;

namespace NoticeBoard.Data
{

    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string adminPassword)
        {
            using (var context = new NoticeBoardDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<NoticeBoardDbContext>>()))
            {
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, adminPassword, "admin@custom.com");
                await EnsureRole(serviceProvider, adminID, NotificationConstants.ContactAdministratorsRole);

                //TODO:ensure more roles

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<ICustomUserManager>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new CustomUser {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                    string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))//ensure that current role doesn't exists and create it
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<ICustomUserManager>();

            var user = await userManager.FindByIdAsync(uid);

            if(user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            
            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(NoticeBoardDbContext context,string adminID)
        {
            var Notifications = new Notification[]
            {
                new Notification()
                {
                    Name = "I want to sell my car",
                    Description="best car in the world, my number is 12345",
                    OwnerID = adminID
                },
                new Notification
                {
                    Name = "Test notification 2",
                    Description=" Test description for 2 notificaton.",
                    OwnerID = adminID
                },
                new Notification
                {
                    Name = "Apple MQAG2B/A iPhone X",
                    Description="Get ready for a stunning experience with this Apple iPhone X smartphone.",
                    OwnerID = adminID
                },
                new Notification
                {
                    Name = "Samsung Galaxy Note10 SM-N970U",
                    Description="Samsung",
                    OwnerID = adminID
                },
                new Notification
                {
                    Name = "Smartwatch Lenovo",
                    Description="Smartwatch Lenovo HW10H BlazeMotorola.",
                    OwnerID = adminID
                }
            };
            foreach(var notif in Notifications)
            {
                var notifInDb = context.Notifications.AsNoTracking().Where(e=> e.Name== notif.Name&& e.OwnerID==notif.OwnerID).SingleOrDefault();
                if(notifInDb==null)//if db does not contain the same item
                {
                    context.Notifications.Add(notif);
                }
            }
            context.SaveChanges();
            
            var Comments = new Comment[]
            {
                new Comment()
                {
                    Id = context.Notifications.AsNoTracking().Single(n=>n.Name==Notifications[0].Name).Id,
                    OwnerID = adminID,
                    Description="Test comment for 0 notification content"
                },
                new Comment()
                {
                    Id = context.Notifications.AsNoTracking().Single(n=>n.Name==Notifications[0].Name).Id,
                    OwnerID = adminID,
                    Description="Test 1 comment for 0 notification content"
                },
                new Comment()
                {
                    Id = context.Notifications.AsNoTracking().Single(n=>n.Name==Notifications[0].Name).Id,
                    OwnerID = adminID,
                    Description="Test 3 comment for 0 notification content"
                },
                new Comment()
                {
                    Id = context.Notifications.AsNoTracking().Single(n=>n.Name==Notifications[1].Name).Id,
                    OwnerID = adminID,
                    Description="Test 1 comment for 1 notification content"
                },
                new Comment()
                {
                    Id = context.Notifications.AsNoTracking().Single(n=>n.Name==Notifications[1].Name).Id,
                    OwnerID = adminID,
                    Description="Test 2 comment for 1 notification content"
                },
                new Comment()
                {
                    Id = context.Notifications.AsNoTracking().Single(n=>n.Name==Notifications[2].Name).Id,
                    OwnerID = adminID,
                    Description="Test 1 comment for 2 notification content"
                }
            };
            foreach(var com in Comments)//add initial data whenever
            {
                var comInDb = context.Comments.AsNoTracking().Where(e=> e.Description== com.Description&& e.OwnerID==com.OwnerID).SingleOrDefault();
                if(comInDb==null)
                {
                    context.Comments.Add(com);
                }
            }
            context.SaveChanges();
        }


    }

}