using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;//installed in source project
using System.Threading.Tasks;
using NoticeBoard.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;

namespace NoticeBoard.XUnitTests
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<SqlServerControllerTests>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public SqlServerControllerTests Fixture { get; }

        public BasicTests(WebApplicationFactory<Startup> factory,
            SqlServerControllerTests fixture)
        {
            _factory = factory;
            Fixture = fixture;
        }
        [Theory]
        [InlineData("/")]
        [InlineData("Notification/Index")]
        public async Task GetEndpointsSuccessAndCorrectContentType(string url)
        {
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync(url);
            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);

        }
        [Fact]
        public async Task GetEndpointsAllNotificationDetailsSuccessAndCorrectContentType()
        {
            string detailsUrl = "Notification/Details/";
            //Arrange
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    var client = _factory.CreateClient();
                    //Act
                    var notifications = await context.Notifications.AsNoTracking().ToListAsync();
                    
                    foreach (var item in notifications)
                    {
                        var result = await client.GetAsync($"{detailsUrl}{item.Id}");
                        //Assert
                        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
                    }
                }
            }
        }
        [Fact]
        public async Task NotAuthorizedUserRequestCreateNotificationsReturnsRedirectToLogin() 
        {
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync("Notification/Create");
            //Assert
            Assert.Equal("/CustomAccount/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }
        [Fact]
        public async Task NotAuthorizedUserRequestDeleteNotificationsReturnsRedirectToLogin()
        {
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync("Notification/Delete");
            //Assert
            Assert.Equal("/CustomAccount/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }
        [Fact]
        public async Task NotAuthorizedUserRequestEditNotificationsReturnsRedirectToLogin()
        {
            //Arrange
            var client = _factory.CreateClient();
            //Act
            var result = await client.GetAsync("Notification/Edit");
            //Assert
            Assert.Equal("/CustomAccount/Login", result.RequestMessage.RequestUri.AbsolutePath);
        }
        //[Fact]
        //public async Task GetEndpointsSuccessFromFormLogin()
        //{
        //    //Arrange
        //    var client = _factory.CreateClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, "CustomAccount/Login");

        //    var nameValueCollection = new List<KeyValuePair<string, string>>();

        //    nameValueCollection.Add(new KeyValuePair<string, string>("Email", "admin@custom.com"));
        //    nameValueCollection.Add(new KeyValuePair<string, string>("Password", "!234Qwer"));
        //    nameValueCollection.Add(new KeyValuePair<string, string>("RememberMe", "true"));
        //    //nameValueCollection.Add(new KeyValuePair<string, string>("__RequestVerificationToken", "CfDJ8CA9GfPi1KtHsC0fLgh6cLSgvo6OhptFvz8-s5CPABYIYkK6gya35yFDj39dAtjSbEEJhgTdI-3l8GqFStTcQjERpyBq20rwfhZY-sLxhOj4fBWn5oiOSAEf7brTbc94aUPbt-OnFLpP7FmZP4f8tu0"));
        //    //nameValueCollection.Add(new KeyValuePair<string, string>("RememberMe", "false"));

        //    //Act
        //    request.Content = new FormUrlEncodedContent(nameValueCollection);
        //    var result = await client.SendAsync(request);

        //    //Assert
        //    Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        //}badRequest Error

        //private static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
        //[Fact]
        //public async Task MiddlewareTest_ReturnsNotFoundForRequest()
        //{
        //    //Arrage
        //    var host = CreateHostBuilder(null).Build();

        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        var customAccountController = new CustomAccountController
        //        (
        //            services.GetRequiredService<ICustomUserManager>(),
        //            services.GetRequiredService<ICustomSignInManager>(),
        //            services.GetRequiredService<ILogger<CustomAccountController>>(),
        //            services.GetRequiredService<IEmailSender>()

        //        );
        //        //Act
        //        var result = await customAccountController.LoginConfirmed(NotificationControllerTests.GetUserViewModel());
        //        //Assert
        //        var viewResult = Assert.IsType<LocalRedirectResult>(result);
        //    }
        //}//httpContext error


        [Theory]
        [InlineData("Notification/Create")]
        [InlineData("Notification/Edit")]
        [InlineData("Notification/Delete")]
        public async Task Get_NotificationPagesRedirectsAnUnauthenticatedUser(string TestUrl)
        {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            // Act
            var response = await client.GetAsync(TestUrl);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/CustomAccount/Login",
                response.Headers.Location.OriginalString);
        }

        [Fact]
        public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            //Act
            var responseCreate = await client.GetAsync("Notification/Create");
            var responseNoOwnNotificationEdit = await client.GetAsync("Notification/Edit/1");
            var responseNoOwnNotificationDelete = await client.GetAsync("Notification/Delete/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseCreate.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, responseNoOwnNotificationEdit.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, responseNoOwnNotificationDelete.StatusCode);
        }


    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
    

}

