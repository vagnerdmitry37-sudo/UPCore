using UPCore.Domain.UpEntities;

namespace UPCore.Application.UpEntities;

public sealed record UpEntitiesPage(IReadOnlyList<UpEntity> Items, int TotalRecords);
