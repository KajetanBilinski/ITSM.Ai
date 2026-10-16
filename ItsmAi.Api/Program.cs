using System.Text.Json.Serialization;
using ItsmAi.Agent;
using ItsmAi.Api.ExceptionHandling;
using ItsmAi.Application;
using ItsmAi.Application.Knowledge.Search;
using ItsmAi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var minimumSimilarity =
    builder.Configuration.GetValue<double>(
        "Rag:MinimumSimilarity",
        -1.0);

if (!double.IsFinite(minimumSimilarity)
    || minimumSimilarity is < -1.0 or > 1.0)
{
    throw new InvalidOperationException(
        "RAG minimum similarity must be between -1 and 1.");
}

builder.Services.AddSingleton(
    new RagSearchOptions(minimumSimilarity));

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAgent();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


