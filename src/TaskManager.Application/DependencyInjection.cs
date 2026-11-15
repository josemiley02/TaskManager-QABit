using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Services;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<CreateWorkItemService>();
        services.AddScoped<GetWorkItemsService>();
        services.AddScoped<GetWorkItemByIdService>();
        services.AddScoped<DeleteWorkItemService>();
        services.AddScoped<UpdateWorkItemService>();
        return services;
    }
}
