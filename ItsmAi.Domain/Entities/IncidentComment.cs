using ItsmAi.Domain.Enums;

namespace ItsmAi.Domain.Entities;

public class IncidentComment
{
    public Guid Id { get; private set; }

    public Guid IncidentId { get; private set; }

    public string Content { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public IncidentCommentAuthorType AuthorType { get; private set; }

    internal IncidentComment(
        Guid incidentId,
        string content,
        IncidentCommentAuthorType authorType)
    {
        Id = Guid.NewGuid();
        IncidentId = incidentId;
        Content = content;
        AuthorType = authorType;
        CreatedAt = DateTime.UtcNow;
    }
}