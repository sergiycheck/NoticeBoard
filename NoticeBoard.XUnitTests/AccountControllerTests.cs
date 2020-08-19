// using System;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using Xunit;
// using NoticeBoard.Data;
// using NoticeBoard.Interfaces;
// using Microsoft.Extensions.Hosting;
// using Microsoft.AspNetCore.Hosting;

// namespace NoticeBoard.XUnitTests//-----------error
// {
//     public class AccountControllerTest:IClassFixture<InMemoryControllerTestsFixture>
//     {
//         public AccountControllerTest(InMemoryControllerTestsFixture fixture)
//         {
      
//             Context = fixture.Context;
//             UserManager = fixture.UserManager;
//             SignInManager = fixture.SignInManager;
//             LoggerFactory = fixture.LoggerFactory;
//             ServiceProvider = fixture.ServiceProvider;
//         }
//         public NoticeBoardDbContext Context { get; }
//         public ICustomUserManager UserManager { get; }

//         public ICustomSignInManager SignInManager { get; }
//         public ILoggerFactory LoggerFactory { get; }

//         public IServiceProvider ServiceProvider { get; }

//         [Fact]
//         public void ContextNotNull()
//         {
//             Assert.NotNull(Context);
//         }

//         [Fact]
//         public void UserManagerNotNull()
//         {
//             Assert.NotNull(UserManager);
//         }

//         [Fact]
//         public void SignInManagerNotNull()
//         {
//             Assert.NotNull(SignInManager);
//         }

//         [Fact]
//         public void LoggerFactoryNotNull()
//         {
//             Assert.NotNull(LoggerFactory);
//         }

//         [Fact]
//         public void ServiceProviderNotNull()
//         {
//             Assert.NotNull(ServiceProvider);
//         }
//     }
// }