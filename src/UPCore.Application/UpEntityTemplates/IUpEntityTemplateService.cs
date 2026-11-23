using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Application.UpEntityTemplates;

public interface IUpEntityTemplateService
{
    Task<CreateUpEntityTemplatesResult> CreateAsync(
        IReadOnlyList<UpEntityTemplate> upEntityTemplates,
        CancellationToken cancellationToken);

    Task<UpEntityTemplate?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<UpEntityTemplatesPage> GetAllAsync(UpEntityTemplatesQuery query, CancellationToken cancellationToken);

    Task<UpdateUpEntityTemplatesResult> UpdateAsync(
        IReadOnlyList<UpEntityTemplate> upEntityTemplates,
        CancellationToken cancellationToken);

    Task<DeleteUpEntityTemplatesResult> DeleteAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken);
}
