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
}