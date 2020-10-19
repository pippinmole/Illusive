using Illusive.Illusive.Database.Behaviour;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Illusive {
    public class Startup {
        public Startup(IConfiguration configuration) {
            this._configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDataProtection();
            services.AddAuthentication(options => {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
            });
            
            services.AddMvc().AddRazorPagesOptions(options => {
                options.Conventions.AddPageRoute("/Forum", "ForumPage/{text?}");
                // options.Conventions.AuthorizeFolder("/");
                // options.Conventions.AllowAnonymousToPage("/Login");
                // options.Conventions.AllowAnonymousToPage("/Logout");
                // options.Conventions.AllowAnonymousToPage("/Signup");
                // options.Conventions.AllowAnonymousToPage("/Index");
            });

            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IForumService, ForumService>();

            services.AddControllers();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}