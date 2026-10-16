using ItsmAi.Application.Abstractions.Knowledge;
using ItsmAi.Application.Incidents.AddComment;
using ItsmAi.Application.Incidents.ChangeStatus;
using ItsmAi.Application.Incidents.Create;
using ItsmAi.Application.Incidents.GetById;
using ItsmAi.Application.Incidents.GetList;
using ItsmAi.Application.Knowledge.Chunking;
using ItsmAi.Application.Knowledge.IngestDocument;
using ItsmAi.Application.Knowledge.Search;
using ItsmAi.Application.Requesters.Create;
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
        services.AddScoped<AddIncidentCommentHandler>();
        services.AddScoped<CreateRequesterHandler>();
        services.AddScoped<IngestKnowledgeDocumentHandler>();
        services.AddScoped<SearchKnowledgeHandler>();

        services.AddSingleton<ITextChunker, WordTextChunker>();

        return services;
    }
}