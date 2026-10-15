using ItsmAi.Agent.Abstractions.Approvals;
using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Agent.Abstractions.Observability;
using ItsmAi.Application.Contracts;
using ItsmAi.Infrastructure.Ai.Approvals;
using ItsmAi.Infrastructure.AI.Observability;
using ItsmAi.Infrastructure.AI.Ollama;
using ItsmAi.Infrastructure.Persistence.Repositories;
using ItsmAi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.EntityFrameworkCore;

namespace ItsmAi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Connection string 'Database' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.UseVector();
                });
        });

        var ollamaBaseUrl =
            configuration["Ollama:BaseUrl"];

        if (string.IsNullOrWhiteSpace(ollamaBaseUrl))
        {
            throw new InvalidOperationException(
                "Ollama BaseUrl is not configured.");
        }

        services.AddHttpClient<
            ILlmProvider,
            OllamaLlmProvider>(
            client =>
            {
                client.BaseAddress =
                    new Uri(ollamaBaseUrl);

                client.Timeout =
                    TimeSpan.FromMinutes(5);
            });

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IRequesterRepository, RequesterRepository>();
        services.AddScoped<IAgentRunStore,EfAgentRunStore>();
        services.AddScoped<IAgentApprovalStore,EfAgentApprovalStore>();

        return services;
    }
}