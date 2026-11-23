using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Application.UpEntityTemplates;

public sealed record CreateUpEntityTemplatesResult(
    IReadOnlyList<UpEntityTemplate> Created,
    IReadOnlyList<string> ExistingTypes);
