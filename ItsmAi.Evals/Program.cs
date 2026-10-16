using ItsmAi.Agent;
using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Orchestration;
using ItsmAi.Application;
using ItsmAi.Application.Knowledge.Search;
using ItsmAi.Evals;
using ItsmAi.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "appsettings.Development.json");

if (!File.Exists(configPath))
{
    throw new FileNotFoundException(
        "Run evaluations from the solution root.",
        configPath);
}

var configuration = new ConfigurationBuilder()
    .AddJsonFile(configPath)
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddLogging();
services.AddApplication();
services.AddInfrastructure(configuration);
services.AddAgent();

services.AddSingleton(
    new RagSearchOptions(
        configuration.GetValue<double>(
            "Rag:MinimumSimilarity",
            -1.0)));

using var serviceProvider =
    services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var llm = scope.ServiceProvider
    .GetRequiredService<ILlmProvider>();

var toolRegistry = scope.ServiceProvider
    .GetRequiredService<IAgentToolRegistry>();

var tools = toolRegistry.Tools
    .Select(tool => new LlmToolDefinition(
        tool.Name,
        tool.Description,
        tool.ParametersJsonSchema,
        tool.Access))
    .ToArray();

var incidentId = Guid.Parse(
    "550e8400-e29b-41d4-a716-446655440000");

var cases = new[]
{
    new EvaluationCase(
        "VPN knowledge search",
        "Znajdź w naszej bazie wiedzy procedurę naprawy VPN.",
        "search_knowledge"),

    new EvaluationCase(
        "Incident details",
        $"Sprawdź incydent o ID {incidentId} i podaj jego status.",
        "get_incident",
        incidentId),

    new EvaluationCase(
        "Incident filtering",
        "Wyszukaj incydenty o priorytecie High.",
        "search_incidents"),

    new EvaluationCase(
        "Write operation",
        $"Dodaj komentarz 'User contacted' do incydentu {incidentId}.",
        "add_incident_comment",
        incidentId),

    new EvaluationCase(
        "Simple greeting",
        "Cześć, przedstaw się krótko.",
        null)
};

var passed = 0;
var failed = 0;

Console.WriteLine("ITSM AI — Model Evaluation");
Console.WriteLine(
    $"Model: {configuration["Ollama:Model"]}");
Console.WriteLine();

foreach (var testCase in cases)
{
    Console.WriteLine($"Running: {testCase.Name}");

    try
    {
        var messages = new[]
        {
            new LlmMessage(
                LlmMessageRole.System,
                AgentSystemPrompt.Value),

            new LlmMessage(
                LlmMessageRole.User,
                testCase.UserMessage)
        };

        using var timeout =
            new CancellationTokenSource(
                TimeSpan.FromMinutes(3));

        var response = await llm.CompleteAsync(
            new LlmRequest(messages, tools),
            timeout.Token);

        var calls = response.ToolCalls;

        var correctTool =
            testCase.ExpectedTool is null
                ? calls.Count == 0
                : calls.Count == 1
                  && calls[0].Name.Equals(
                      testCase.ExpectedTool,
                      StringComparison.OrdinalIgnoreCase);

        var correctArguments = true;

        if (correctTool &&
            testCase.ExpectedIncidentId is Guid expectedId)
        {
            var arguments = calls[0].Arguments;

            correctArguments =
                arguments.ValueKind ==
                    System.Text.Json.JsonValueKind.Object
                && arguments.TryGetProperty(
                    "incidentId",
                    out var idProperty)
                && Guid.TryParse(
                    idProperty.GetString(),
                    out var actualId)
                && actualId == expectedId;
        }

        var success = correctTool && correctArguments;

        if (success)
        {
            passed++;
            Console.WriteLine("  PASS");
        }
        else
        {
            failed++;
            Console.WriteLine("  FAIL");
        }

        Console.WriteLine(
            $"  Expected: {testCase.ExpectedTool ?? "none"}");

        Console.WriteLine(
            $"  Actual: {string.Join(", ",
                calls.Select(x => x.Name))}");

        if (!correctArguments)
        {
            Console.WriteLine(
                "  IncidentId argument does not match.");
        }
    }
    catch (Exception exception)
    {
        failed++;

        Console.WriteLine("  ERROR");
        Console.WriteLine(
            $"  {exception.Message}");
    }

    Console.WriteLine();
}

var total = passed + failed;

var accuracy = total == 0
    ? 0
    : (double)passed / total * 100;

Console.WriteLine("EVALUATION SUMMARY");
Console.WriteLine($"Passed: {passed}");
Console.WriteLine($"Failed: {failed}");
Console.WriteLine($"Accuracy: {accuracy:F1}%");

if (failed > 0)
{
    Environment.ExitCode = 1;
}