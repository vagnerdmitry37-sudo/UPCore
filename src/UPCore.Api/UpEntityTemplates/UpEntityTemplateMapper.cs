using Riok.Mapperly.Abstractions;

using UPCore.Application.UpEntityTemplates;
using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Api.UpEntityTemplates;

[Mapper]
internal static partial class UpEntityTemplateMapper
{
    [MapperIgnoreTarget(nameof(UpEntityTemplate.Id))]
    public static partial UpEntityTemplate ToUpEntityTemplate(this CreateUpEntityTemplateRequest request);

    [MapperIgnoreTarget(nameof(UpEntityTemplate.Type))]
    public static partial UpEntityTemplate ToUpEntityTemplate(this UpdateUpEntityTemplateRequest request);

    public static partial UpEntityTemplateResponse ToResponse(this UpEntityTemplate upEntityTemplate);

    public static partial IReadOnlyList<UpEntityTemplateResponse> ToResponses(this IReadOnlyList<UpEntityTemplate> upEntityTemplates);

    public static partial UpEntityTemplatesQuery ToQuery(this GetUpEntityTemplatesRequest request);

    public static partial UpEntityTemplatesPageResponse ToResponse(this UpEntityTemplatesPage page);
}
