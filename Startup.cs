using System;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using ChristianMihai.AspNetCoreThrottler;
using Illusive.Database;
using Illusive.Illusive.Core.Database.Interfaces;
using Illusive.Illusive.Core.Mail.Behaviour;
using Illusive.Illusive.Core.Mail.Interfaces;
using Illusive.Illusive.Core.Mail.Options;
using Illusive.Illusive.Core.User_Management.Behaviour;
using Illusive.Illusive.Core.User_Management.Extension_Methods;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbGenericRepository;
using reCAPTCHA.AspNetCore;
using Westwind.AspNetCore.Markdown;

namespace Illusive {
    public class Startup {
        
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) {
            this._configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDataProtection();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.LoginPath = new PathString("/Login");
                    options.LogoutPath = new PathString("/Logout");
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                });
            
            services.AddAuthorization(options => {
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("IsAdmin"));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson();

            services.AddRecaptcha(this._configuration.GetSection("RecaptchaSettings"));
            services.AddAntiforgery(options => {
                options.HeaderName = "XSRF-TOKEN";
            });

            var mongoDbIdentity = new MongoDbIdentityConfiguration {
                MongoDbSettings = new MongoDbSettings {
                    ConnectionString = this._configuration.GetConnectionString("DatabaseConnectionString"),
                    DatabaseName = "IllusiveDatabase"
                },
                IdentityOptionsAction = options => {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;

                    // ApplicationUser settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
                }
            };

            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentity);
            
            services.Configure<MailSenderOptions>(this._configuration.GetSection(MailSenderOptions.Name));
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 5;
            });

            services.Configure<RateLimitOptions>(config => {
                config.RequestRateMs = 2000;
                config.LimitSoft = 4;
                config.LimitHard = 5;
                config.HardLimitMessage = "You are requesting too frequently... Refresh this page to continue.";
            });

            services.AddScoped<IAppUserManager, AppUserManager>();
            services.AddSingleton<IMailSender, MailSender>();
            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddSingleton<IForumService, ForumService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IContentService, ContentService>();

            services.AddMarkdown();

            services.AddControllers();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/NotFound");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMarkdown();
            app.UseRouting();
            
            app.UseThrottling();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}