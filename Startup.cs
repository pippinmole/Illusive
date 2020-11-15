using Illusive.Database;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            services.AddAuthentication(options => {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => {
                // options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.AccessDeniedPath = "/Index";
                options.SlidingExpiration = true;
            });

            services.AddMvc().AddRazorPagesOptions(options => {
                // options.Conventions.AddPageRoute("/Forum", "ForumPage/{text?}");
                // options.Conventions.AuthorizeFolder("/");
                // options.Conventions.AllowAnonymousToPage("/Login");
                // options.Conventions.AllowAnonymousToPage("/Logout");
                // options.Conventions.AllowAnonymousToPage("/Signup");
                // options.Conventions.AllowAnonymousToPage("/Index");
            }).SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson();

            services.AddAuthorization(options => {
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("IsAdmin"));
            });

            services.AddRecaptcha(this._configuration.GetSection("RecaptchaSettings"));

            services.AddAntiforgery(options => {
                options.HeaderName = "XSRF-TOKEN";
            });
            
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 5;
            });

            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IForumService, ForumService>();
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
            
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
            });
        }
    }
}