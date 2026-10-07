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

        Title = title;
        Description = description;
        Priority = priority;

        Status = IncidentStatus.New;
        CreatedAt = DateTime.UtcNow;
    }
}