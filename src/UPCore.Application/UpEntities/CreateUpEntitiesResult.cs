using UPCore.Domain.UpEntities;

namespace UPCore.Application.UpEntities;

public sealed record CreateUpEntitiesResult(IReadOnlyList<UpEntity> Created, IReadOnlyList<UpEntityError> Errors);
