using UPCore.Domain.UpEntities;

namespace UPCore.Application.UpEntities;

public sealed record UpdateUpEntitiesResult(
    IReadOnlyList<UpEntity> Updated,
    IReadOnlyList<Guid> MissingIds,
    IReadOnlyList<UpEntityError> Errors);
