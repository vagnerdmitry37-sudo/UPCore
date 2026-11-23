using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace UPCore.Api.UpEntityTemplates;

public sealed record GetUpEntityTemplatesRequest(
    [property: FromQuery(Name = "first")][Range(0, int.MaxValue)] int First = 0,
    [property: FromQuery(Name = "rows")][Range(1, GetUpEntityTemplatesRequest.MaxRows)] int Rows = GetUpEntityTemplatesRequest.DefaultRows)
{
    public const int DefaultRows = 50;
    public const int MaxRows = 200;
}
