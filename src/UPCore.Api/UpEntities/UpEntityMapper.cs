using System.Text.Json;

using Riok.Mapperly.Abstractions;

using UPCore.Application.UpEntities;
using UPCore.Domain.UpEntities;

namespace UPCore.Api.UpEntities;

[Mapper]
internal static partial class UpEntityMapper
{
    [MapperIgnoreTarget(nameof(UpEntity.Id))]
    [MapProperty(nameof(CreateUpEntityRequest.Data), nameof(UpEntity.Data), Use = nameof(SerializeData))]
    public static partial UpEntity ToUpEntity(this CreateUpEntityRequest request);

    [MapperIgnoreTarget(nameof(UpEntity.Type))]
    [MapProperty(nameof(UpdateUpEntityRequest.Data), nameof(UpEntity.Data), Use = nameof(SerializeData))]
    public static partial UpEntity ToUpEntity(this UpdateUpEntityRequest request);

    [MapProperty(nameof(UpEntity.Data), nameof(UpEntityResponse.Data), Use = nameof(ParseData))]
    public static partial UpEntityResponse ToResponse(this UpEntity upEntity);

    public static partial IReadOnlyList<UpEntityResponse> ToResponses(this IReadOnlyList<UpEntity> upEntities);

    public static partial UpEntitiesQuery ToQuery(this GetUpEntitiesRequest request);

    public static partial UpEntitiesPageResponse ToResponse(this UpEntitiesPage page);

    private static string SerializeData(JsonElement data) => data.GetRawText();

    private static JsonElement ParseData(string data) => JsonSerializer.Deserialize<JsonElement>(data);
}
