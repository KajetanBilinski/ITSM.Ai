using System.Text.Json;

namespace ItsmAi.Api.Contracts.Agent;

public record ExecuteToolRequest(
    string Tool,
    JsonElement Arguments);