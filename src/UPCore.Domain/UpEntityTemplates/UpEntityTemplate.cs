namespace UPCore.Domain.UpEntityTemplates;

public sealed class UpEntityTemplate
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public List<UpEntityField> Fields { get; set; } = [];
}
