using System.ComponentModel.DataAnnotations;

using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Api.UpEntityTemplates;

public sealed record UpEntityFieldRequest(
    [Required, MaxLength(100), RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Key must be letters and digits, starting with a letter, for example brand or year.")]
    string Key,
    [Required, MaxLength(200)] string Label,
    [EnumDataType(typeof(UpEntityFieldKind))] UpEntityFieldKind Kind,
    bool Required);
