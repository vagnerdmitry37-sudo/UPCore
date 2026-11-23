namespace UPCore.Api.UpEntityTemplates;

public sealed record UpEntityTemplatesPageResponse(IReadOnlyList<UpEntityTemplateResponse> Items, int TotalRecords);
