using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Application.UpEntityTemplates;

public sealed record UpEntityTemplatesPage(IReadOnlyList<UpEntityTemplate> Items, int TotalRecords);
