using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Api.UpEntityTemplates;

public sealed record UpEntityFieldResponse(string Key, string Label, UpEntityFieldKind Kind, bool Required);
