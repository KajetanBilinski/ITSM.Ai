namespace ItsmAi.Domain.Entities;

public class IncidentComment
{
    public Guid Id { get; private set; }

    public Guid IncidentId { get; private set; }

    public string Content { get; private set; }

    public DateTime CreatedAt { get; private set; }

    internal IncidentComment(
        Guid incidentId,
        string content)
    {
        Id = Guid.NewGuid();
        IncidentId = incidentId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
}