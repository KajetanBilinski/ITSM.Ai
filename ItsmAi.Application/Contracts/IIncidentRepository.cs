using ItsmAi.Domain.Entities;
using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Contracts;

public interface IIncidentRepository
{
    Task AddAsync(
        Incident incident,
        CancellationToken cancellationToken = default);

    Task<Incident?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<Incident?> GetForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Incident> Items, int TotalCount)> GetPagedAsync(
    IncidentStatus? status,
    IncidentPriority? priority,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default);

    Task AddCommentAsync(
    IncidentComment comment,
    CancellationToken cancellationToken = default);
}