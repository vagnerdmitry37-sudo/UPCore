using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace UPCore.Api.UpEntities;

public sealed record GetUpEntitiesRequest(
    [property: FromQuery(Name = "type")][Required, MaxLength(100)] string Type,
    [property: FromQuery(Name = "first")][Range(0, int.MaxValue)] int First = 0,
    [property: FromQuery(Name = "rows")][Range(1, GetUpEntitiesRequest.MaxRows)] int Rows = GetUpEntitiesRequest.DefaultRows)
{
    public const int DefaultRows = 50;
    public const int MaxRows = 200;
}
