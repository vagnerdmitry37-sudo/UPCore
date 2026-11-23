using System.ComponentModel.DataAnnotations;

namespace UPCore.Api.UpEntityTemplates;

public sealed record CreateUpEntityTemplateRequest(
    [Required, MaxLength(100), RegularExpression("^[a-z][a-z0-9-]*$", ErrorMessage = "Type must be lower-case kebab-case, for example car or pet-dog.")]
    string Type,
    [Required, MinLength(1), MaxLength(100)] IReadOnlyList<UpEntityFieldRequest> Fields)
    : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Fields.DistinctBy(field => field.Key).Count() != Fields.Count)
        {
            yield return new ValidationResult("Each field key can appear only once.", [nameof(Fields)]);
        }
    }
}
