using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Tools;
using ItsmAi.Agent.Tools.Incidents;
using Microsoft.Extensions.DependencyInjection;

namespace ItsmAi.Agent;

public static class DependencyInjection
{
    public static IServiceCollection AddAgent(
        this IServiceCollection services)
    {
        services.AddScoped<IAgentTool, GetIncidentTool>();
        services.AddScoped<IAgentTool, SearchIncidentsTool>();
        services.AddScoped<IAgentToolRegistry, AgentToolRegistry>();

        return services;
    }
}