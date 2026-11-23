using Microsoft.EntityFrameworkCore;

using UPCore.Application.UpEntities;
using UPCore.Domain.UpEntities;
using UPCore.Domain.UpEntityTemplates;
using UPCore.Infrastructure.Persistence;

namespace UPCore.Infrastructure.UpEntities;

internal sealed class UpEntityService(AppDbContext dbContext) : IUpEntityService
{
    public async Task<CreateUpEntitiesResult> CreateAsync(
        IReadOnlyList<UpEntity> upEntities,
        CancellationToken cancellationToken)
    {
        Dictionary<string, UpEntityTemplate> templatesByType = await FindTemplatesAsync(
            upEntities.Select(upEntity => upEntity.Type),
            cancellationToken);

        List<UpEntityError> errors = [];
        for (int index = 0; index < upEntities.Count; index++)
        {
            UpEntity upEntity = upEntities[index];

            if (!templatesByType.TryGetValue(upEntity.Type, out UpEntityTemplate? upEntityTemplate))
            {
                errors.Add(new UpEntityError(index, nameof(UpEntity.Type), $"Type {upEntity.Type} has no template."));
                continue;
            }

            errors.AddRange(ValidateData(index, upEntityTemplate, upEntity.Data));
        }

        if (errors.Count > 0)
        {
            return new CreateUpEntitiesResult([], errors);
        }

        foreach (UpEntity upEntity in upEntities)
        {
            upEntity.Id = Guid.CreateVersion7();
        }

        dbContext.UpEntities.AddRange(upEntities);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUpEntitiesResult(upEntities, []);
    }

    public async Task<UpEntity?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.UpEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(upEntity => upEntity.Id == id, cancellationToken);

    public async Task<UpEntitiesPage> GetAllAsync(UpEntitiesQuery query, CancellationToken cancellationToken)
    {
        IQueryable<UpEntity> matching = dbContext.UpEntities
            .AsNoTracking()
            .Where(upEntity => upEntity.Type == query.Type);

        int totalRecords = await matching.CountAsync(cancellationToken);
        List<UpEntity> items = await matching
            .OrderBy(upEntity => upEntity.Id)
            .Skip(query.First)
            .Take(query.Rows)
            .ToListAsync(cancellationToken);

        return new UpEntitiesPage(items, totalRecords);
    }

    public async Task<UpdateUpEntitiesResult> UpdateAsync(
        IReadOnlyList<UpEntity> upEntities,
        CancellationToken cancellationToken)
    {
        List<Guid> ids = [.. upEntities.Select(upEntity => upEntity.Id)];
        Dictionary<Guid, UpEntity> existingById = await dbContext.UpEntities
            .Where(upEntity => ids.Contains(upEntity.Id))
            .ToDictionaryAsync(upEntity => upEntity.Id, cancellationToken);
        List<Guid> missingIds = [.. ids.Where(id => !existingById.ContainsKey(id))];

        if (missingIds.Count > 0)
        {
            return new UpdateUpEntitiesResult([], missingIds, []);
        }

        Dictionary<string, UpEntityTemplate> templatesByType = await FindTemplatesAsync(
            existingById.Values.Select(upEntity => upEntity.Type),
            cancellationToken);

        List<UpEntityError> errors = [];
        for (int index = 0; index < upEntities.Count; index++)
        {
            UpEntity existing = existingById[upEntities[index].Id];
            UpEntityTemplate upEntityTemplate = templatesByType.GetValueOrDefault(existing.Type)
                ?? throw new InvalidOperationException($"UpEntity {existing.Id} has no template {existing.Type}.");

            errors.AddRange(ValidateData(index, upEntityTemplate, upEntities[index].Data));
        }

        if (errors.Count > 0)
        {
            return new UpdateUpEntitiesResult([], [], errors);
        }

        foreach (UpEntity upEntity in upEntities)
        {
            existingById[upEntity.Id].Data = upEntity.Data;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateUpEntitiesResult([.. ids.Select(id => existingById[id])], [], []);
    }

    public async Task<IReadOnlyList<Guid>> DeleteAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        List<Guid> existingIds = await dbContext.UpEntities
            .Where(upEntity => ids.Contains(upEntity.Id))
            .Select(upEntity => upEntity.Id)
            .ToListAsync(cancellationToken);
        List<Guid> missingIds = [.. ids.Except(existingIds)];

        if (missingIds.Count > 0)
        {
            return missingIds;
        }

        await dbContext.UpEntities
            .Where(upEntity => ids.Contains(upEntity.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return [];
    }

    private static IEnumerable<UpEntityError> ValidateData(int index, UpEntityTemplate upEntityTemplate, string data) =>
        UpEntityDataValidator.Validate(upEntityTemplate, data)
            .Select(message => new UpEntityError(index, nameof(UpEntity.Data), message));

    private async Task<Dictionary<string, UpEntityTemplate>> FindTemplatesAsync(
        IEnumerable<string> types,
        CancellationToken cancellationToken)
    {
        List<string> distinctTypes = [.. types.Distinct()];

        return await dbContext.UpEntityTemplates
            .AsNoTracking()
            .Where(upEntityTemplate => distinctTypes.Contains(upEntityTemplate.Type))
            .ToDictionaryAsync(upEntityTemplate => upEntityTemplate.Type, cancellationToken);
    }
}
