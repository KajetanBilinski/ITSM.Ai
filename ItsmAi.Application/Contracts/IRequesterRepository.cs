using ItsmAi.Domain.Entities;

namespace ItsmAi.Application.Contracts;

public interface IRequesterRepository
{
    Task AddAsync(
        Requester requester,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<Requester?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);
}