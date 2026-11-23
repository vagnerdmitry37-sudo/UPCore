using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using UPCore.Application.UpEntities;
using UPCore.Domain.UpEntities;

namespace UPCore.Api.UpEntities;

[ApiController]
[Route(UpEntityRoutes.Base)]
public sealed class UpEntitiesController(IUpEntityService upEntityService) : ControllerBase
{
    private const int MaxUpEntitiesPerRequest = 100;

    [HttpPost]
    [ProducesResponseType<IReadOnlyList<UpEntityResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<UpEntityResponse>>> Create(
        [MinLength(1), MaxLength(MaxUpEntitiesPerRequest)] IReadOnlyList<CreateUpEntityRequest> requests,
        CancellationToken cancellationToken)
    {
        CreateUpEntitiesResult result = await upEntityService.CreateAsync(
            [.. requests.Select(request => request.ToUpEntity())],
            cancellationToken);

        if (result.Errors.Count > 0)
        {
            return ErrorsProblem(result.Errors);
        }

        return StatusCode(StatusCodes.Status201Created, result.Created.ToResponses());
    }

    [HttpGet(UpEntityRoutes.ById)]
    [ProducesResponseType<UpEntityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpEntityResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        UpEntity? upEntity = await upEntityService.GetAsync(id, cancellationToken);

        if (upEntity is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"UpEntity {id} not found.");
        }

        return Ok(upEntity.ToResponse());
    }

    [HttpGet]
    [ProducesResponseType<UpEntitiesPageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpEntitiesPageResponse>> GetAll(
        [FromQuery] GetUpEntitiesRequest request,
        CancellationToken cancellationToken)
    {
        UpEntitiesPage page = await upEntityService.GetAllAsync(request.ToQuery(), cancellationToken);

        return Ok(page.ToResponse());
    }

    [HttpPut]
    [ProducesResponseType<IReadOnlyList<UpEntityResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<UpEntityResponse>>> Update(
        [MinLength(1), MaxLength(MaxUpEntitiesPerRequest)] IReadOnlyList<UpdateUpEntityRequest> requests,
        CancellationToken cancellationToken)
    {
        if (requests.DistinctBy(request => request.Id).Count() != requests.Count)
        {
            ModelState.AddModelError(string.Empty, "Each UpEntity can appear only once.");
            return ValidationProblem(ModelState);
        }

        UpdateUpEntitiesResult result = await upEntityService.UpdateAsync(
            [.. requests.Select(request => request.ToUpEntity())],
            cancellationToken);

        if (result.MissingIds.Count > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: $"UpEntities not found: {string.Join(", ", result.MissingIds)}");
        }

        if (result.Errors.Count > 0)
        {
            return ErrorsProblem(result.Errors);
        }

        return Ok(result.Updated.ToResponses());
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromQuery, Required, MinLength(1), MaxLength(MaxUpEntitiesPerRequest)] Guid[] ids,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Guid> missingIds = await upEntityService.DeleteAsync([.. ids.Distinct()], cancellationToken);

        if (missingIds.Count > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: $"UpEntities not found: {string.Join(", ", missingIds)}");
        }

        return NoContent();
    }

    private ActionResult ErrorsProblem(IReadOnlyList<UpEntityError> errors)
    {
        foreach (UpEntityError error in errors)
        {
            ModelState.AddModelError($"[{error.Index}].{error.Field}", error.Message);
        }

        return ValidationProblem(ModelState);
    }
}
