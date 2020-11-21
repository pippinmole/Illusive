using System;
using ChristianMihai.AspNetCoreThrottler;
using Illusive.Database;
using Illusive.Illusive.Core.Database.Interfaces;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddMvc().AddRazorPagesOptions(options => {
                // options.Conventions.AddPageRoute("/Forum", "ForumPage/{text?}");
                // options.Conventions.AuthorizeFolder("/");
                // options.Conventions.AllowAnonymousToPage("/Login");
                // options.Conventions.AllowAnonymousToPage("/Logout");
                // options.Conventions.AllowAnonymousToPage("/Signup");
                // options.Conventions.AllowAnonymousToPage("/Index");
            }).SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson();

            services.AddRecaptcha(this._configuration.GetSection("RecaptchaSettings"));

            services.AddAntiforgery(options => {
                options.HeaderName = "XSRF-TOKEN";
            });
            
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

            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddSingleton<IAccountService, AccountService>();
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
            });
        }
    }
}