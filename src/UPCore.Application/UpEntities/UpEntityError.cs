namespace UPCore.Application.UpEntities;

public sealed record UpEntityError(int Index, string Field, string Message);
