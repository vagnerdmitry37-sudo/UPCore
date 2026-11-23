using System.Text.Json;

namespace UPCore.Domain.UpEntityTemplates;

public static class UpEntityDataValidator
{
    public static IReadOnlyList<string> Validate(UpEntityTemplate upEntityTemplate, string data)
    {
        using JsonDocument document = JsonDocument.Parse(data);
        JsonElement values = document.RootElement;
        List<string> errors = [];

        HashSet<string> knownKeys = [.. upEntityTemplate.Fields.Select(field => field.Key)];
        foreach (JsonProperty property in values.EnumerateObject())
        {
            if (!knownKeys.Contains(property.Name))
            {
                errors.Add($"{upEntityTemplate.Type} has no field {property.Name}.");
            }
        }

        foreach (UpEntityField field in upEntityTemplate.Fields)
        {
            string? error = Check(field, values);
            if (error is not null)
            {
                errors.Add(error);
            }
        }

        return errors;
    }

    private static string? Check(UpEntityField field, JsonElement values)
    {
        if (!values.TryGetProperty(field.Key, out JsonElement value) || value.ValueKind == JsonValueKind.Null)
        {
            return field.Required ? $"Enter {field.Key}." : null;
        }

        return field.Kind switch
        {
            UpEntityFieldKind.Text => CheckText(field, value),
            UpEntityFieldKind.Number => value.ValueKind == JsonValueKind.Number ? null : $"{field.Key} must be a number.",
            _ => $"{field.Key} has an unsupported kind.",
        };
    }

    private static string? CheckText(UpEntityField field, JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.String)
        {
            return $"{field.Key} must be text.";
        }

        return field.Required && string.IsNullOrWhiteSpace(value.GetString()) ? $"Enter {field.Key}." : null;
    }
}
