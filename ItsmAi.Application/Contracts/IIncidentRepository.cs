using ItsmAi.Domain.Entities;

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
}