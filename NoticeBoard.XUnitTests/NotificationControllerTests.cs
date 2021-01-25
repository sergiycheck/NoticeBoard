using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using NoticeBoard.Controllers;
using NoticeBoard.Tests;
using System;
using Xunit;
using System.Security.Claims;
using System.Threading.Tasks;
using NoticeBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using NoticeBoard.Authorization;
using Microsoft.Extensions.Logging;
using NoticeBoard.Repositories;
using NoticeBoard.Helpers;
using NSubstitute;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NoticeBoard.Interfaces;
using NoticeBoard.Models.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using NuGet.Frameworks;
using NoticeBoard.Data;
using NoticeBoard.Helpers.CustomMapper;


//in directory NoticeBoard.XUnitTest execute following command to add reference to testing project
//dotnet add ./NoticeBoard.XUnitTests.csproj reference ../NoticeBoard/NoticeBoard.csproj
//add needed packages

namespace NoticeBoard.XUnitTests
{
    public class NotificationControllerTests : IClassFixture<SqlServerControllerTests>
    {
        public NotificationControllerTests(SqlServerControllerTests fixture) => Fixture = fixture;
        public SqlServerControllerTests Fixture { get; }

        //cannot use CreateHostBuilder because of configuration, connection strings and long pileline

        private CustomUser GetCustomUser(NoticeBoardDbContext context, int id) => new CustomUser()
        {
            Email = "testEmail@gmail",
            Id = context.Notifications.AsNoTracking().FirstOrDefault(n => n.Id == id).OwnerID,
            EmailConfirmed = true,
            FirstName = "testFirsName",
            LastName = "testLastName",
            UserName = "testUser"
        };

        [Theory]
        [InlineData(1)]
        public async Task TestIdexReturnViewResult(int id)
        {
            //TODO:Implement customAuthorizationService for testing. Currently getting error on mocking AuthorizeAsync
            //http://blog.stoverud.no/posts/how-to-unit-test-asp-net-core-authorizationhandler/
            //https://issue.life/questions/42965727
            //Arrange
            using (var transaction = Fixture.Connection.BeginTransaction())
            {

                using (var context = Fixture.CreateContext(transaction))
                {
                    var mockAuthorizationService = new Mock<ICustomAuthorizationService>();
                    //var mockAuthorizationResult = new Mock<AuthorizationResult>();
                    //mockAuthorizationResult.SetupGet(r => r.Succeeded).Returns(true);
                    var mockCustomUserManager = new Mock<ICustomUserManager>();
                    var customUser = GetCustomUser(context, id);
                    mockCustomUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(customUser);
                    var mockLogger = new Mock<ILogger<DI_BaseController>>();
                    ILogger<DI_BaseController> logger = mockLogger.Object;
                    var mockUser = new Mock<ClaimsPrincipal>();
                    mockUser.SetupGet(user => user.Identity.IsAuthenticated).Returns(true);


                    mockAuthorizationService.Setup(
                        service => service.AuthorizeAsync(
                            mockUser.Object,
                            context.Notifications.AsNoTracking().FirstOrDefault(),
                            NotificatinOperations.Update)).ReturnsAsync(AuthorizationResult.Success());

                    var controller = new NotificationController(
                    mockAuthorizationService.Object,
                    mockCustomUserManager.Object,
                    logger,
                    new NotificationsRepository(context));

                    //Act
                    var result = await controller.Details(id);

                    //Assert
                    var viewResult = Assert.IsType<ViewResult>(result);
                    var model = Assert.IsAssignableFrom<NotificationViewModel>(viewResult.Model);
                    Assert.Equal(context.Notifications
                        .AsNoTracking()
                        .Include(n=>n.Comments)
                        .FirstOrDefault(n=>n.Id==id)
                        .Comments.Count(), model.CommentViewModel.Count());

                }
            }
        }

        [Fact]
        public async Task DetailsReturnViewResult()
        {
            //Arrange
            using (var transaction = Fixture.Connection.BeginTransaction())
            {

                using (var context = Fixture.CreateContext(transaction))
                {
                    var mockAuthorizationService = new Mock<ICustomAuthorizationService>();

                    var mockCustomUserManager = new Mock<ICustomUserManager>();

                    var mockLogger = new Mock<ILogger<DI_BaseController>>();
                    ILogger<DI_BaseController> logger = mockLogger.Object;
                    var mockUser = new Mock<ClaimsPrincipal>();
                    mockUser.SetupGet(user => user.Identity.IsAuthenticated).Returns(true);


                    mockAuthorizationService.Setup(
                        service => service.AuthorizeAsync(
                            mockUser.Object,
                            context.Notifications.AsNoTracking().FirstOrDefault(),
                            NotificatinOperations.Update)).ReturnsAsync(AuthorizationResult.Success());

                    var controller = new NotificationController(
                    mockAuthorizationService.Object,
                    mockCustomUserManager.Object,
                    logger,
                    new NotificationsRepository(context));

                    //Act
                    var result = await controller.Index();

                    //Assert
                    var viewResult = Assert.IsType<ViewResult>(result);
                    var model = Assert.IsAssignableFrom<IEnumerable<Notification>>(viewResult.Model);
                    Assert.Equal(context.Notifications.AsNoTracking().Count(), model.Count());

                }
            }
        }

        [Fact]
        public async Task LoginAsAdminTestAllPagesReturnViewResult()
        {
            //Arrange
            using (var transaction = Fixture.Connection.BeginTransaction())
            {

                using (var context = Fixture.CreateContext(transaction))
                {
                    var adminModel = GetAdminViewModel();
                    var customUser = new CustomUser()
                    {
                        UserName = adminModel.Email,
                        Email = adminModel.Email,
                        EmailConfirmed = true
                    };
                    var mockCustomUserManager = new Mock<ICustomUserManager>();
                    mockCustomUserManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>()))
                        .ReturnsAsync(customUser);
                    var mockCustomSignInManager = new Mock<ICustomSignInManager>();


                    mockCustomSignInManager.Setup(manager=>
                        manager.PasswordSignInAsync(
                            customUser,
                            adminModel.Password,
                            true,
                            false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
                    var mockEmailSender = new Mock<IEmailSender>();

                    var mockLogger = new Mock<ILogger<CustomAccountController>>();
                    ILogger<CustomAccountController> logger = mockLogger.Object;

                    var mockUser = new Mock<ClaimsPrincipal>();
                    mockUser.SetupGet(user => user.Identity.IsAuthenticated).Returns(true);

                    var mockMapper = new Mock<ICustomMapper>();
                    mockMapper.Setup(mapper => mapper.Map_EditUserViewModel(It.IsAny<CustomUser>())).Returns(new EditUserViewModel());



                    var controller = new CustomAccountController(
                    mockCustomUserManager.Object,
                    mockCustomSignInManager.Object,
                    logger,
                    mockEmailSender.Object,
                    mockMapper.Object);
                    

                    //Act
                    var result = await controller.LoginConfirmed(adminModel);

                    //Assert
                    var viewResult = Assert.IsType<LocalRedirectResult>(result);

                }
            }
        }
        public static LoginViewModel GetAdminViewModel() => 
            new LoginViewModel()
            {
                Email = "admin@custom.com",
                Password = "!234Qwer",
                RememberMe = true,
                ReturnUrl="/"
            };
        public static LoginViewModel GetUserViewModel() =>
            new LoginViewModel()
            {
                Email = "user@mail.com",
                Password = "!234Qwer",
                RememberMe = true,
                ReturnUrl = "/"
            };

        [Theory]
        [InlineData("06bfe098-c686-4a8b-ad51-accd61f965d3")]//admin id
        public void CreateReturnsViewWithUserId(string AdminId)
        {
            //Arrange
            using (var transaction = Fixture.Connection.BeginTransaction())
            {

                using (var context = Fixture.CreateContext(transaction))
                {
                    var mockAuthorizationService = new Mock<ICustomAuthorizationService>();

                    var mockCustomUserManager = new Mock<ICustomUserManager>();
                    mockCustomUserManager.Setup(m => m.GetUserId(
                        It.IsAny<ClaimsPrincipal>())).Returns(AdminId);

                    var mockLogger = new Mock<ILogger<DI_BaseController>>();
                    ILogger<DI_BaseController> logger = mockLogger.Object;

                    var mockUser = new Mock<ClaimsPrincipal>();
                    mockUser.SetupGet(user => user.Identity.IsAuthenticated).Returns(true);


                    mockAuthorizationService.Setup(
                        service => service.AuthorizeAsync(
                            mockUser.Object,
                            context.Notifications.AsNoTracking().FirstOrDefault(),
                            NotificatinOperations.Update)).ReturnsAsync(AuthorizationResult.Success());

                    var controller = new NotificationController(
                    mockAuthorizationService.Object,
                    mockCustomUserManager.Object,
                    logger,
                    new NotificationsRepository(context));

                    //another way of initializing readonly properties
                    //https://hudosvibe.net/post/mock-user-identity-in-asp.net-mvc-core-controller-tests
                    controller.ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = mockUser.Object
                        }
                    };

                    //Act
                    var result = controller.Create();

                    //Assert
                    var viewResult = Assert.IsType<ViewResult>(result);
                    Assert.Equal(AdminId, controller.ViewBag.UserId);


                }
            }
        }




    }
}
