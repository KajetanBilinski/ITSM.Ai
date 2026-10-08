using ItsmAi.Domain.Enums;

namespace ItsmAi.Domain.Entities;

public class Incident
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public IncidentStatus Status { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Incident(
    string title,
    string description,
    IncidentPriority priority)
    {
        Id = Guid.NewGuid();

        Number = $"INC-{Id.ToString("N")[..8].ToUpperInvariant()}";

        Title = title;
        Description = description;
        Priority = priority;

        Status = IncidentStatus.New;
        CreatedAt = DateTime.UtcNow;
    }

    public void StartProgress()
    {
        if (Status != IncidentStatus.New)
            throw new InvalidOperationException(
                "Only new incidents can be started.");

        Status = IncidentStatus.InProgress;
    }

    public void Resolve()
    {
        if (Status != IncidentStatus.InProgress)
            throw new InvalidOperationException(
                "Only incidents in progress can be resolved.");

        Status = IncidentStatus.Resolved;
    }

    public void Close()
    {
        if (Status != IncidentStatus.Resolved)
            throw new InvalidOperationException(
                "Only resolved incidents can be closed.");

        Status = IncidentStatus.Closed;
    }

    public void ChangePriority(IncidentPriority priority)
    {
        if (Status == IncidentStatus.Closed)
            throw new InvalidOperationException(
                "Closed incident cannot be modified.");

        Priority = priority;
    }
}