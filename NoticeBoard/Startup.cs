using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using NoticeBoard.Authorization;
using NoticeBoard.Helpers;
using Microsoft.AspNetCore.Http;
using NoticeBoard.Interfaces;
using NoticeBoard.Repositories;
using NoticeBoard.AuthorizationsManagers;
using NoticeBoard.Models;
using NoticeBoard.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using NoticeBoard.Helpers.CustomMapper;
using NoticeBoard.Helpers.CustomUserValidator;

namespace NoticeBoard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<NoticeBoardDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("AzureNoticeBoardDb")));//NoticeBoardDb

            services.AddDefaultIdentity<CustomUser>
                (options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<CustomRole>()//added roles
                .AddEntityFrameworkStores<NoticeBoardDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();//https://stackoverflow.com/questions/58203748/is-live-reload-with-in-process-aspnet-core-3-possible

           services.AddRazorPages().AddRazorRuntimeCompilation();
           

            services.AddTransient<ICommentsRepository,CommentsRepository>();
            services.AddTransient<INotificationsRepository,NotificationsRepository>();
            services.AddTransient<IEmailSender, MyCustomEmailSender>();
            services.AddTransient<ICustomMapper, CustomMapper>();

            services.AddTransient<ICustomUserRepository, CustomUserRepository>();
            services.AddTransient<Helpers.CustomUserValidator.IUserValidator<CustomUser>, CustomUserValidator>();




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
                options.User.RequireUniqueEmail = true;

                //options.SignIn.RequireConfirmedAccount
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = new PathString("/Areas/CustomAuthorization/Views/CustomAccount/Login");//"/Identity/Account/Login";//change here
                options.AccessDeniedPath = new PathString("/Areas/CustomAuthorization/Views/CustomAccount/AccessDenied");;//"/Identity/Account/AccessDenied";
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

            services.AddScoped<ICustomUserManager,CustomUserManager>();
            services.AddScoped<ICustomSignInManager,CustomSignInManager>();
            services.AddScoped<ICustomRoleManager, CustomRoleManager>();
            services.AddScoped<ICustomRolesRespository, CustomRolesRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/Error";
                    await next();
                }
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                _ = app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            //for js compilation
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "MyAreaCustomAuthorization",
                    areaName: "CustomAuthorization",
                    pattern: "CustomAuthorization/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Notification}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
