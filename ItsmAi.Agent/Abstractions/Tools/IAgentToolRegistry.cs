namespace ItsmAi.Agent.Abstractions.Tools;

public interface IAgentToolRegistry
{
    IReadOnlyCollection<IAgentTool> Tools { get; }

    IAgentTool? Get(string name);
}