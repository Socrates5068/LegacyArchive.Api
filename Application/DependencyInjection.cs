using FluentValidation;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Services;
using LegacyArchive.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyArchive.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IArtworkService, ArtworkService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddValidatorsFromAssemblyContaining<CreateArtworkRequestValidator>();

        return services;
    }
}
