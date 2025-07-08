using Microsoft.OpenApi.Models;

namespace HealthBeside.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HealthBeside.API API",
                Version = "v1"
            });
        });

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthBeside.API v1");
                c.RoutePrefix = string.Empty; 
            });
        }

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.Run();
    }
}