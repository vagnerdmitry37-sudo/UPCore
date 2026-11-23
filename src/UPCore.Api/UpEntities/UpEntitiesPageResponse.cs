namespace UPCore.Api.UpEntities;

public sealed record UpEntitiesPageResponse(IReadOnlyList<UpEntityResponse> Items, int TotalRecords);
