using ItsmAi.Agent.Abstractions.Tools;

namespace ItsmAi.Agent.Tools;

public class AgentToolRegistry : IAgentToolRegistry
{
    private readonly IReadOnlyDictionary<string, IAgentTool> _tools;

    public AgentToolRegistry(
        IEnumerable<IAgentTool> tools)
    {
        _tools = tools.ToDictionary(
            x => x.Name,
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<IAgentTool> Tools =>
        _tools.Values.ToList();

    public IAgentTool? Get(string name)
    {
        return _tools.GetValueOrDefault(name);
    }
}