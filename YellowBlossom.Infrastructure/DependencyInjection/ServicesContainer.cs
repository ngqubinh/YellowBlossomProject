using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using YellowBlossom.Application.Interfaces.Auth;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Application.Interfaces.Setting;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Repositories.Auth;
using YellowBlossom.Infrastructure.Repositories.PMIS;
using YellowBlossom.Infrastructure.Repositories.Setting;

namespace YellowBlossom.Infrastructure.DependencyInjection
{
    public static class ServicesContainer
    {
        public static IServiceCollection MainService(this IServiceCollection services, IConfiguration config)
        {
            string dbType = config["DatabaseType"]!;
            string connectString;

            switch (dbType.ToLower())
            {
                case "postgreSQL":
                    connectString = config.GetConnectionString("postgreSQL")!;
                    if (string.IsNullOrEmpty(connectString))
                        throw new ArgumentException("PostgreSQL connection string is missing.");
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(connectString));
                    break;

                case "SQLSERVER":
                    connectString = config.GetConnectionString("SQLSERVER")!;
                    if (string.IsNullOrEmpty(connectString))
                        throw new ArgumentException("SQLSERVER connection string is missing.");
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectString));
                    break;

                default:
                    connectString = config.GetConnectionString("postgreSQL")!;
                    if (string.IsNullOrEmpty(connectString))
                        throw new ArgumentException("PostgreSQL connection string is missing (default fallback).");
                    services.AddDbContext<ApplicationDbContext>(options =>
                      options.UseNpgsql(connectString));
                    Console.WriteLine($"Unknown DatabaseType '{dbType}', defaulting to PostgreSQL.");
                    break;
            }

            // CORS Configuration
            services.AddCors(options =>
            {
                options.AddPolicy("client_cors", builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
                });
            });

            // JWT Configuration
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = false,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = config["JWT:Issuer"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!))
                  };
              });

            // Identity Configuration
            services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
              .AddRoles<IdentityRole>()
              .AddRoleManager<RoleManager<IdentityRole>>()
              .AddUserManager<UserManager<User>>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddSignInManager<SignInManager<User>>()
              .AddDefaultTokenProviders();

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddRazorPages();

            // Services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInitService, InitService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IBugService, BugService>();

            return services;
        }
    }
}
