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


//in directory NoticeBoard.XUnitTest execute following command to add reference to testing project
//dotnet add ./NoticeBoard.XUnitTests.csproj reference ../NoticeBoard/NoticeBoard.csproj
//add needed packages

namespace NoticeBoard.XUnitTests
{
    public class NotificationControllerTests : IClassFixture<SqlServerControllerTests>
    {
        public NotificationControllerTests(SqlServerControllerTests fixture) => Fixture = fixture;
        public SqlServerControllerTests Fixture { get; }
        [Fact]
        public async Task TestIdexReturnViewResult()
        {
            //TODO:Implement customAuthorizationService for testing. Currently getting error on mocking AuthorizeAsync
            //http://blog.stoverud.no/posts/how-to-unit-test-asp-net-core-authorizationhandler/
            //https://issue.life/questions/42965727
            //Arrage
            using (var transaction = Fixture.Connection.BeginTransaction())
            {

                using (var context = Fixture.CreateContext(transaction))
                {
                    var mockAuthorizationService = new Mock<IAuthorizationService>();
                    var mockUserManager = new Mock<UserManager<IdentityUser>>();
                    //var mockAuthorizationResult = new Mock<AuthorizationResult>();
                    //mockAuthorizationResult.SetupGet(r => r.Succeeded).Returns(true);
                    var mockUser = new Mock<ClaimsPrincipal>();
                    mockUser.SetupGet(user => user.Identity.IsAuthenticated).Returns(true);


                    mockAuthorizationService.Setup(
                        service => service.AuthorizeAsync(
                            mockUser.Object,
                            context.Notifications.AsNoTracking().FirstOrDefault(),
                            NotificatinOperations.Update)).ReturnsAsync(AuthorizationResult.Success());
                    var controller = new NotificationController(context, mockAuthorizationService.Object,It.IsAny<UserManager<IdentityUser>>());

                    //Act
                    var result = await controller.Index();

                    //Assert
                    var viewResult = Assert.IsType<ViewResult>(result);
                    var model = Assert.IsAssignableFrom<IEnumerable<Notification>>(viewResult.Model);
                    Assert.Equal(context.Notifications.AsNoTracking().Count(), model.Count());

                }
            }
        }
    }
}
