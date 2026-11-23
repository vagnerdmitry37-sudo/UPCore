namespace UPCore.Application.UpEntityTemplates;

public sealed record DeleteUpEntityTemplatesResult(IReadOnlyList<Guid> MissingIds, IReadOnlyList<string> TypesInUse);
