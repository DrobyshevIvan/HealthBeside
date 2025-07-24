using System.Text;
using HealthBeside.API.Handlers;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Services;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Shared;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure;
using HealthBeside.Infrastructure.Options;
using HealthBeside.Infrastructure.Processors;
using HealthBeside.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace HealthBeside.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection(JwtOptions.JwtOptionsKey));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", opts =>
            {
                opts.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:5180");
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("HealthBesideDbConnectionString")
            )
        );

        // Processors containers
        builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();

        // Repositories containers
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IForumCommentRepository, ForumCommentRepository>();
        builder.Services.AddScoped<IForumPostRepository, ForumPostRepository>();
        builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services containers
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IForumPostService, ForumPostService>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        var jwt = builder.Configuration.GetSection("JwtOptions");

        var secretKey = builder.Configuration.GetValue<string>("JwtOptions:Secret");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie().AddGoogle(options =>
        {
            var clientId = builder.Configuration["Authentication:Google:ClientId"];
            if (clientId == null)
                throw new ArgumentNullException(nameof(clientId));
            
            var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            if (clientSecret == null)
                throw new ArgumentNullException(nameof(clientId));
            
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.JwtOptionsKey)
                .Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["ACCESS_TOKEN"];
                    return Task.CompletedTask;
                }
            };

        });

        // Add services to the container.

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("JWT Authentication API");
            });
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/scalar/", permanent: false);
                return Task.CompletedTask;
            });
        }
        
        app.UseCors("CorsPolicy");

        builder.Services.AddAuthorization();

        builder.Services.AddHttpContextAccessor();

        app.UseExceptionHandler("/Error");

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}