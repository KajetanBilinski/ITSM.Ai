using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Entities;
using ItsmAi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ItsmAi.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly AppDbContext _dbContext;

    public IncidentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Incident incident,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Incidents.AddAsync(
            incident,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Incident?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Incidents
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Incident?> GetForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Incidents
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Incident> Items, int TotalCount)> GetPagedAsync(
    IncidentStatus? status,
    IncidentPriority? priority,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Incidents
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => x.Priority == priority.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}