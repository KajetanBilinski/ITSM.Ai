using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Application.Contracts;
using ItsmAi.Infrastructure.Ai.Ollama;
using ItsmAi.Infrastructure.Persistence.Repositories;
using ItsmAi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ItsmAi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

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

        return services;
    }
}