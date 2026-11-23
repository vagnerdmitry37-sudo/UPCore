using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Application.UpEntityTemplates;

public sealed record UpdateUpEntityTemplatesResult(
    IReadOnlyList<UpEntityTemplate> Updated,
    IReadOnlyList<Guid> MissingIds);
