using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoticeBoard.Authorization;
using NoticeBoard.AuthorizationsManagers;
using NoticeBoard.Controllers;
using NoticeBoard.Data;
using NoticeBoard.Helpers;
using NoticeBoard.Interfaces;
using NoticeBoard.Models;
using NoticeBoard.Models.ViewModels;
using NoticeBoard.Repositories;
using NoticeBoard.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoticeBoard.XUnitTests
{
    public class MiddleWareTestsWithControllers : IClassFixture<SqlServerControllerTests>
    {
        private IHost _host;
        private LoginViewModel[] _loginViewModels;
        public SqlServerControllerTests Fixture { get; }
        private SignInManager<CustomUser> _signInManager { get; set; }
        public MiddleWareTestsWithControllers(SqlServerControllerTests fixture) 
        {
            Fixture = fixture;
            _loginViewModels = new LoginViewModel[]
            {
                new LoginViewModel()
                {
                    Email = "admin@custom.com",
                    Password = "!234Qwer",
                    RememberMe = true,
                    ReturnUrl="/"
                },
                new LoginViewModel()
                {
                    Email = "user@mail.com",
                    Password = "!234Qwer",
                    RememberMe = true,
                    ReturnUrl = "/"
                },
                new LoginViewModel()
                {
                    Email = "user1@mail.com",
                    Password = "!234Qwerco",
                    RememberMe = true,
                    ReturnUrl = "/"
                },
                new LoginViewModel()
                {
                    Email = "user2@mail.com",
                    Password = "!234Qwercoco",
                    RememberMe = true,
                    ReturnUrl = "/"
                }
            };

        }
        [Fact]
        public async Task StarWebHost() 
        {
            using (var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddDbContext<NoticeBoardDbContext>
                            (o => o.UseSqlServer(SqlServerControllerTests.ConnectionString));

                            services.AddDefaultIdentity<CustomUser>(options => options.SignIn.RequireConfirmedAccount = true)
                                            .AddRoles<IdentityRole>()//added roles
                                            .AddEntityFrameworkStores<NoticeBoardDbContext>();

                            services.AddControllersWithViews().AddRazorRuntimeCompilation();//https://stackoverflow.com/questions/58203748/is-live-reload-with-in-process-aspnet-core-3-possible
                            services.AddRazorPages().AddRazorRuntimeCompilation();

                            services.AddTransient<ICommentsRepository, CommentsRepository>();
                            services.AddTransient<INotificationsRepository, NotificationsRepository>();



                            services.AddControllers(config =>//add require authenticated user use [AllowAnonymous] for controller actions
                                        {
                                            // using Microsoft.AspNetCore.Mvc.Authorization;
                                            // using Microsoft.AspNetCore.Authorization;
                                            var policy = new AuthorizationPolicyBuilder()
                                                .RequireAuthenticatedUser()
                                                .Build();
                                            config.Filters.Add(new AuthorizeFilter(policy));
                                        });


                            services.Configure<IdentityOptions>(options =>
                            {

                                options.Password.RequireDigit = true;
                                options.Password.RequireLowercase = true;
                                options.Password.RequireNonAlphanumeric = true;
                                options.Password.RequireUppercase = true;
                                options.Password.RequiredLength = 7;
                                options.Password.RequiredUniqueChars = 1;

                                // Lockout settings.
                                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                                options.Lockout.MaxFailedAccessAttempts = 10;
                                options.Lockout.AllowedForNewUsers = true;

                                // User settings.
                                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                                options.User.RequireUniqueEmail = false;

                                //options.SignIn.RequireConfirmedAccount
                            });

                            services.ConfigureApplicationCookie(options =>
                            {
                                // Cookie settings
                                options.Cookie.HttpOnly = true;
                                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                                options.LoginPath = new PathString("/CustomAccount/Login");//"/Identity/Account/Login";//change here
                                options.AccessDeniedPath = new PathString("/CustomAccount/AccessDenied"); ;//"/Identity/Account/AccessDenied";
                                options.SlidingExpiration = true;
                            });

                            // Authorization handlers.
                            services.AddScoped<IAuthorizationHandler,
                                    NoticetIsOwnerAuthorizationHandler>();//services that uses entity framework core must be registred
                                                                          //usign AddScoped

                            //TODO:try to implement customAuthorizationSerivce for better testing (descriptor error)
                            services.AddScoped<ICustomAuthorizationService, CustomAuthorizationService>();

                            services.AddSingleton<IAuthorizationHandler,
                                                NoticeAdministratorAuthorizationHandler>();

                            services.AddScoped<ICustomUserManager, CustomUserManager>();
                            services.AddScoped<ICustomSignInManager, CustomSignInManager>();
                        })
                        .Configure(async app =>
                        {
                            //await Initialize(app.ApplicationServices,
                            //    _loginViewModels[0].Password,
                            //    _loginViewModels[1].Password,
                            //    _loginViewModels[2].Password);
                            //Microsoft.AspNetCore.Builder.UseExtensions.Use()
                            app.Use(async (context, next) => //https://stackoverflow.com/questions/53514318/auto-login-on-debug-asp-net-core-2-1
                            {
                                _signInManager = context.RequestServices.GetRequiredService<SignInManager<CustomUser>>();
                            });
                        });
                })
                .StartAsync())
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var customAccountController = new CustomAccountController
                    (
                        services.GetRequiredService<ICustomUserManager>(),
                        services.GetRequiredService<ICustomSignInManager>(),
                        services.GetRequiredService<ILogger<CustomAccountController>>(),
                        services.GetRequiredService<IEmailSender>()

                    );
                    //Arrange
                    var userManager = services.GetService<ICustomUserManager>();
                    //var singInManager = services.GetService<ICustomSignInManager>();


                    //Act
                    var user = await userManager.FindByEmailAsync(_loginViewModels[1].Email);
                    //Assert
                    Assert.NotNull(user);
                    //Act
                    var res = await _signInManager.PasswordSignInAsync(user, _loginViewModels[1].Password, true, false);
                    Assert.Equal(Microsoft.AspNetCore.Identity.SignInResult.Success.ToString(), res.ToString());
                    //var result = await customAccountController.LoginConfirmed(_loginViewModels[1]);
                    ////Assert
                    //var viewResult = Assert.IsType<LocalRedirectResult>(result);
                }
            }



        }
        private async Task Initialize(IServiceProvider serviceProvider, 
            string adminPassword,
            string user1Password,
            string user2Password)
        {
            using (var context = new NoticeBoardDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<NoticeBoardDbContext>>()))
            {
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, adminPassword, _loginViewModels[0].Email);
                var user1ID = await EnsureUser(serviceProvider, user1Password, _loginViewModels[1].Email);
                var user2ID = await EnsureUser(serviceProvider, user2Password, _loginViewModels[2].Email);
                await EnsureRole(serviceProvider, adminID, NotificationConstants.ContactAdministratorsRole);

                //TODO:ensure more roles

                SeedDB(context, adminID,user1ID,user2ID);
            }
        }


        private async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                            string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<ICustomUserManager>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new CustomUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
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

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        private void SeedDB(NoticeBoardDbContext context, string adminID,string user1Id,string user2Id)
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
                    OwnerID = user1Id
                },
                new Notification
                {
                    Name = "Samsung Galaxy Note10 SM-N970U",
                    Description="Samsung",
                    OwnerID = user1Id
                },
                new Notification
                {
                    Name = "Smartwatch Lenovo",
                    Description="Smartwatch Lenovo HW10H BlazeMotorola.",
                    OwnerID = user2Id
                }
            };
            if (!context.Notifications.Any())
            {
                context.Notifications.AddRange(Notifications);
                context.SaveChanges();
            }            

            var Comments = new Comment[]
            {
                new Comment()
                {
                    OwnerID = adminID,
                    Description="Test comment for 0 notification content"
                },
                new Comment()
                {

                    OwnerID = user1Id,
                    Description="Test 1 comment for 0 notification content"
                },
                new Comment()
                {

                    OwnerID = adminID,
                    Description="Test 3 comment for 0 notification content"
                },
                new Comment()
                {

                    OwnerID = user1Id,
                    Description="Test 1 comment for 1 notification content"
                },
                new Comment()
                {
                    OwnerID = adminID,
                    Description="Test 2 comment for 1 notification content"
                },
                new Comment()
                {
                    OwnerID = user2Id,
                    Description="Test 1 comment for 2 notification content"
                }
            };
            if (!context.Comments.Any())
            {
                context.Comments.AddRange(Comments);
                context.SaveChanges();
            }            
        }



    }
}
