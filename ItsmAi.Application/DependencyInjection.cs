using ItsmAi.Application.Incidents.ChangeStatus;
using ItsmAi.Application.Incidents.Create;
using ItsmAi.Application.Incidents.GetById;
using ItsmAi.Application.Incidents.GetList;
using Microsoft.Extensions.DependencyInjection;

namespace ItsmAi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateIncidentHandler>();
        services.AddScoped<GetIncidentHandler>();
        services.AddScoped<ChangeIncidentStatusHandler>();
        services.AddScoped<GetIncidentsHandler>();

        return services;
    }
}