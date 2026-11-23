using UPCore.Domain.UpEntities;

namespace UPCore.Application.UpEntities;

public interface IUpEntityService
{
    Task<CreateUpEntitiesResult> CreateAsync(IReadOnlyList<UpEntity> upEntities, CancellationToken cancellationToken);

    Task<UpEntity?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<UpEntitiesPage> GetAllAsync(UpEntitiesQuery query, CancellationToken cancellationToken);

    Task<UpdateUpEntitiesResult> UpdateAsync(IReadOnlyList<UpEntity> upEntities, CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> DeleteAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken);
}
