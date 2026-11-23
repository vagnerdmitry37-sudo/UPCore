using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace UPCore.Api.UpEntities;

public sealed record UpdateUpEntityRequest(Guid Id, JsonElement Data) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Data.ValueKind != JsonValueKind.Object)
        {
            yield return new ValidationResult("Data must be an object.", [nameof(Data)]);
        }
    }
}
