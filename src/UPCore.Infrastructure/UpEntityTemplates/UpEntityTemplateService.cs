using Microsoft.EntityFrameworkCore;

using UPCore.Application.UpEntityTemplates;
using UPCore.Domain.UpEntityTemplates;
using UPCore.Infrastructure.Persistence;

namespace UPCore.Infrastructure.UpEntityTemplates;

internal sealed class UpEntityTemplateService(AppDbContext dbContext) : IUpEntityTemplateService
{
    public async Task<CreateUpEntityTemplatesResult> CreateAsync(
        IReadOnlyList<UpEntityTemplate> upEntityTemplates,
        CancellationToken cancellationToken)
    {
        List<string> types = [.. upEntityTemplates.Select(upEntityTemplate => upEntityTemplate.Type)];
        List<string> existingTypes = await dbContext.UpEntityTemplates
            .Where(existing => types.Contains(existing.Type))
            .Select(existing => existing.Type)
            .ToListAsync(cancellationToken);

        if (existingTypes.Count > 0)
        {
            return new CreateUpEntityTemplatesResult([], existingTypes);
        }

        foreach (UpEntityTemplate upEntityTemplate in upEntityTemplates)
        {
            upEntityTemplate.Id = Guid.CreateVersion7();
        }

        dbContext.UpEntityTemplates.AddRange(upEntityTemplates);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUpEntityTemplatesResult(upEntityTemplates, []);
    }

    public async Task<UpEntityTemplate?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.UpEntityTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(upEntityTemplate => upEntityTemplate.Id == id, cancellationToken);

    public async Task<UpEntityTemplatesPage> GetAllAsync(
        UpEntityTemplatesQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<UpEntityTemplate> matching = dbContext.UpEntityTemplates.AsNoTracking();

        int totalRecords = await matching.CountAsync(cancellationToken);
        List<UpEntityTemplate> items = await matching
            .OrderBy(upEntityTemplate => upEntityTemplate.Type)
            .Skip(query.First)
            .Take(query.Rows)
            .ToListAsync(cancellationToken);

        return new UpEntityTemplatesPage(items, totalRecords);
    }

    public async Task<UpdateUpEntityTemplatesResult> UpdateAsync(
        IReadOnlyList<UpEntityTemplate> upEntityTemplates,
        CancellationToken cancellationToken)
    {
        List<Guid> ids = [.. upEntityTemplates.Select(upEntityTemplate => upEntityTemplate.Id)];
        Dictionary<Guid, UpEntityTemplate> existingById = await dbContext.UpEntityTemplates
            .Where(upEntityTemplate => ids.Contains(upEntityTemplate.Id))
            .ToDictionaryAsync(upEntityTemplate => upEntityTemplate.Id, cancellationToken);
        List<Guid> missingIds = [.. ids.Where(id => !existingById.ContainsKey(id))];

        if (missingIds.Count > 0)
        {
            return new UpdateUpEntityTemplatesResult([], missingIds);
        }

        foreach (UpEntityTemplate upEntityTemplate in upEntityTemplates)
        {
            existingById[upEntityTemplate.Id].Fields = upEntityTemplate.Fields;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateUpEntityTemplatesResult([.. ids.Select(id => existingById[id])], []);
    }

    public async Task<DeleteUpEntityTemplatesResult> DeleteAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        List<UpEntityTemplate> existingTemplates = await dbContext.UpEntityTemplates
            .Where(upEntityTemplate => ids.Contains(upEntityTemplate.Id))
            .ToListAsync(cancellationToken);
        List<Guid> missingIds = [.. ids.Except(existingTemplates.Select(upEntityTemplate => upEntityTemplate.Id))];

        if (missingIds.Count > 0)
        {
            return new DeleteUpEntityTemplatesResult(missingIds, []);
        }

        List<string> types = [.. existingTemplates.Select(upEntityTemplate => upEntityTemplate.Type)];
        List<string> typesInUse = await dbContext.UpEntities
            .Where(upEntity => types.Contains(upEntity.Type))
            .Select(upEntity => upEntity.Type)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (typesInUse.Count > 0)
        {
            return new DeleteUpEntityTemplatesResult([], typesInUse);
        }

        dbContext.UpEntityTemplates.RemoveRange(existingTemplates);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteUpEntityTemplatesResult([], []);
    }
}
