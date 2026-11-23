namespace UPCore.Domain.UpEntityTemplates;

public sealed class UpEntityField
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public UpEntityFieldKind Kind { get; set; }

    public bool Required { get; set; }
}
