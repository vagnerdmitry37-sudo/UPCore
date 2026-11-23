namespace UPCore.Api.UpEntityTemplates;

public sealed record UpEntityTemplateResponse(Guid Id, string Type, IReadOnlyList<UpEntityFieldResponse> Fields);
