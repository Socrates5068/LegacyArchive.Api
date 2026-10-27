using Application.Interfaces;
using Application.Services;
using LegacyArchive.Infrastructure.Persistence;
using LegacyArchive.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegacyArchive.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        builder.Configuration.GetConnectionString(
                            "DefaultConnection"));
                });

            builder.Services.AddScoped<IArtworkRepository, ArtworkRepository>();
            builder.Services.AddScoped<ArtworkService>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
