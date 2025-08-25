using System.Globalization;
using System.Text;
using HealthBeside.API.Handlers;
using HealthBeside.API.Middlewares;
using HealthBeside.Application.Extensions;
using HealthBeside.Application.Services;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure;
using HealthBeside.Infrastructure.Configurations.MarketConfiguration;
using HealthBeside.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Stripe;

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
                    .WithOrigins("http://localhost:5173");
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("HealthBesideDbConnectionString")
            )
        );

        //instead of other DI containers, we implement method from class, that include all of containers
        builder.Services.AddProjectServices();

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

        if (secretKey == null)
            throw new ArgumentNullException(nameof(secretKey));


        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie().AddGoogle(options =>
        {
            var clientId = builder.Configuration["Authentication:Google:ClientId"];

            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

            if (clientSecret == null)
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

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

        builder.Services.AddAuthorization();

        builder.Services.AddHttpContextAccessor();

        // Add services to the container.

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddHostedService<OrderTimeoutCleanupBackgroundService>();
        
        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        var stripeSettings = builder.Configuration.GetSection("Stripe").Get<StripeSettings>()!;
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;
        
        // StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("HealthBeside API");
            });
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/scalar/", permanent: false);
                return Task.CompletedTask;
            });
        }

        app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        //app.UseMiddleware<TaskCancellationHandlingMiddleware>(); //TODO fix the middleware to handle task cancellation properly

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

// Загальні tod o:
// TODO: Реалізувати бекграунд сервіс
// TODO: Підключити sandbox оплату через stripe