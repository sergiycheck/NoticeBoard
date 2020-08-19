// using System;
// using NoticeBoard.Data;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using NoticeBoard.Interfaces;
// using NoticeBoard.Authorization;
// using NoticeBoard.AuthorizationsManagers;
// using Microsoft.AspNetCore.Authorization;
// using NoticeBoard.Helpers;
// using Microsoft.AspNetCore.Identity.UI;
// using Microsoft.AspNetCore.Mvc;


// namespace NoticeBoard.XUnitTests
// {
//     public class InMemoryControllerTestsFixture : IDisposable
//     {
//         public InMemoryControllerTestsFixture()
//         {
//             var services = new ServiceCollection();
//             services.AddEntityFrameworkInMemoryDatabase()
//                 .AddDbContext<NoticeBoardDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

//             services.AddDefaultIdentity<IdentityUser>()
//                 .AddEntityFrameworkStores<NoticeBoardDbContext>();----------------------------------------------//object reference not set to an instance of an object

//             services.AddLogging();
//                                     // Authorization handlers.
//             services.AddScoped<IAuthorizationHandler,
//                                 NoticetIsOwnerAuthorizationHandler>();//services that uses entity framework core must be registred
//                                                                       //usign AddScoped

//             //TODO:try to implement customAuthorizationSerivce for better testing (descriptor error)
//             services.AddScoped<ICustomAuthorizationService, CustomAuthorizationService>();

//             services.AddSingleton<IAuthorizationHandler,
//                                 NoticeAdministratorAuthorizationHandler>();

//             services.AddScoped<ICustomUserManager,CustomUserManager>();
//             services.AddScoped<ICustomSignInManager,CustomSignInManager>();

//             // Taken from https://github.com/aspnet/MusicStore/blob/dev/test/MusicStore.Test/ManageControllerTest.cs (and modified)
//             // IHttpContextAccessor is required for SignInManager, and UserManager
//             var context = new DefaultHttpContext();
//             services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor {HttpContext = context});

//             var serviceProvider = services.BuildServiceProvider();

//             Context = serviceProvider.GetService<NoticeBoardDbContext>();
//             Context.Database.EnsureCreated();

//             UserManager = serviceProvider.GetService<ICustomUserManager>();

//             SignInManager = serviceProvider.GetService<ICustomSignInManager>();

//             LoggerFactory = serviceProvider.GetService<ILoggerFactory>();

//             ServiceProvider = serviceProvider;
//         }

//         public NoticeBoardDbContext Context { get; }
//         public ICustomUserManager UserManager { get; }

//         public ICustomSignInManager SignInManager { get; }
//         public ILoggerFactory LoggerFactory { get; }

//         public IServiceProvider ServiceProvider { get; }

//         public void Dispose()
//         {
//             Context.Database.EnsureDeleted();
//         }
//     }
// }