namespace UPCore.Domain.UpEntities;

public sealed class UpEntity
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Data { get; set; } = "{}";
}
