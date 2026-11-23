using System.Text.Json;

namespace UPCore.Api.UpEntities;

public sealed record UpEntityResponse(Guid Id, string Type, JsonElement Data);
