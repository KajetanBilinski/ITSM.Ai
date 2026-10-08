using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItsmAi.Infrastructure.Persistence.Repositories;

public class RequesterRepository : IRequesterRepository
{
    private readonly AppDbContext _dbContext;

    public RequesterRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Requester requester,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Requesters.AddAsync(
            requester,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Requesters
            .AsNoTracking()
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Requesters
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == id,
                cancellationToken);
    }
}