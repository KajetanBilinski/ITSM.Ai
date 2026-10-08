using ItsmAi.Domain.Enums;
using ItsmAi.Domain.Exceptions;

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

    public void ChangeStatus(IncidentStatus newStatus)
    {
        switch (newStatus)
        {
            case IncidentStatus.New:
                throw new DomainException(
                    "Incident cannot be changed back to New.");

            case IncidentStatus.InProgress:
                StartProgress();
                break;

            case IncidentStatus.Resolved:
                Resolve();
                break;

            case IncidentStatus.Closed:
                Close();
                break;

            default:
                throw new DomainException(
                    "Requested incident status transition is not supported.");
        }
    }

    public void StartProgress()
    {
        if (Status != IncidentStatus.New)
            throw new DomainException(
                "Only new incidents can be started.");

        Status = IncidentStatus.InProgress;
    }

    public void Resolve()
    {
        if (Status != IncidentStatus.InProgress)
            throw new DomainException(
                "Only incidents in progress can be resolved.");

        Status = IncidentStatus.Resolved;
    }

    public void Close()
    {
        if (Status != IncidentStatus.Resolved)
            throw new DomainException(
                "Only resolved incidents can be closed.");

        Status = IncidentStatus.Closed;
    }

    public void ChangePriority(IncidentPriority priority)
    {
        if (Status == IncidentStatus.Closed)
            throw new DomainException(
                "Closed incident cannot be modified.");

        Priority = priority;
    }
}