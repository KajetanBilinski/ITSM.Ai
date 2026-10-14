using ItsmAi.Agent.Abstractions.Agent;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Orchestration;
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
        services.AddScoped<IAgentTool, AddIncidentCommentTool>();

        services.AddScoped<IAgentToolRegistry, AgentToolRegistry>();

        services.AddScoped<IAgentOrchestrator,AgentOrchestrator>();

        return services;
    }
}