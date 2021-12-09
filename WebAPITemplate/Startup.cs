using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPITemplate.Controllers;
using WebAPITemplate.Entities;
using WebAPITemplate.Helpers;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;
using WebAPITemplate.Services;

namespace WebAPITemplate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private SymmetricSecurityKey _signingKey;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtIssuerOptions:SecurityKey"]));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITestController, TestController>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IAuthService, AuthService>();            
            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddDbContext<AuthContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AuthDBConnection"), b => b.MigrationsAssembly("Member.HealthEdge.Konnect.API")));
            services.AddControllers();

            // add identity
            var builder = services.AddIdentityCore<IdentityUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<AuthContext>().AddDefaultTokenProviders();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            services.AddSwaggerDocument(document =>
            {
                document.Title = ".Net Core Web API Template";
                document.Version = "v1";
                document.DocumentName = "Web.API.Template";
                document.Description = "Web.API.Template";
                document.PostProcess = (doc =>
                {
                    doc.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Company Name",
                        Email = "eApps@partnershiphp.org",
                        Url = "WebSite URL"
                    };
                });
                document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                document.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT", new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}. You can get a JWT token from /Login/Token."
                }));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPITemplate", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            ConfigureNSwag(app);
        }
        private void ConfigureNSwag(IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
