using System;
using System.IO;
using System.Reflection;
using System.Text;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

            var key = Encoding.UTF8.GetBytes(this._configuration["Jwt:Secret"]);
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.LoginPath = new PathString("/Login");
                    options.LogoutPath = new PathString("/Logout");
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                }).AddJwtBearer(x => {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuers = new[] { this._configuration["Jwt:Issuer"] },
                        ValidAudiences = new[] { this._configuration["Jwt:Issuer"] },
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false
                    };
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

            // AutoMapper setup
            // Injects the service for use in DI
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentity);
            
            services.Configure<MailSenderOptions>(this._configuration.GetSection(MailSenderOptions.Name));

            services.Configure<RateLimitOptions>(config => {
                config.RequestRateMs = 2000;
                config.LimitSoft = 6;
                config.LimitHard = 10;
                config.HardLimitMessage = "You are requesting too frequently... Refresh this page to continue.";
            });

            services.AddScoped<IAppUserManager, AppUserManager>();
            services.AddSingleton<IMailSender, MailSender>();
            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddSingleton<IForumService, ForumService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IContentService, ContentService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "Illusive Public API",
                    Description = "A REST Web API to get data from Illusive forum services.",
                    // TermsOfService = new Uri(""),
                    License = new OpenApiLicense {
                        Name = "Use under MIT",
                        Url = new Uri("https://github.com/pippinmole/Illusive/blob/main/LICENSE"),
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            
            services.AddMarkdown();

            services.AddControllers();
            services.AddRazorPages();
        }
        
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
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(a => { });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Illusive API v1");
                c.DocumentTitle = "Illusive API v1";
                c.RoutePrefix = "api/v1";
                c.InjectStylesheet("/css/swagger-themes/theme-muted.css"); // wwwroot implied
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}