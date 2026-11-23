using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using UPCore.Application.UpEntityTemplates;
using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Api.UpEntityTemplates;

[ApiController]
[Route(UpEntityTemplateRoutes.Base)]
public sealed class UpEntityTemplatesController(IUpEntityTemplateService upEntityTemplateService) : ControllerBase
{
    private const int MaxUpEntityTemplatesPerRequest = 100;

    [HttpPost]
    [ProducesResponseType<IReadOnlyList<UpEntityTemplateResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyList<UpEntityTemplateResponse>>> Create(
        [MinLength(1), MaxLength(MaxUpEntityTemplatesPerRequest)] IReadOnlyList<CreateUpEntityTemplateRequest> requests,
        CancellationToken cancellationToken)
    {
        if (requests.DistinctBy(request => request.Type).Count() != requests.Count)
        {
            ModelState.AddModelError(string.Empty, "Each type can appear only once.");
            return ValidationProblem(ModelState);
        }

        CreateUpEntityTemplatesResult result = await upEntityTemplateService.CreateAsync(
            [.. requests.Select(request => request.ToUpEntityTemplate())],
            cancellationToken);

        if (result.ExistingTypes.Count > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: $"UpEntityTemplates already exist: {string.Join(", ", result.ExistingTypes)}");
        }

        return StatusCode(StatusCodes.Status201Created, result.Created.ToResponses());
    }

    [HttpGet(UpEntityTemplateRoutes.ById)]
    [ProducesResponseType<UpEntityTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpEntityTemplateResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        UpEntityTemplate? upEntityTemplate = await upEntityTemplateService.GetAsync(id, cancellationToken);

        if (upEntityTemplate is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"UpEntityTemplate {id} not found.");
        }

        return Ok(upEntityTemplate.ToResponse());
    }

    [HttpGet]
    [ProducesResponseType<UpEntityTemplatesPageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpEntityTemplatesPageResponse>> GetAll(
        [FromQuery] GetUpEntityTemplatesRequest request,
        CancellationToken cancellationToken)
    {
        UpEntityTemplatesPage page = await upEntityTemplateService.GetAllAsync(request.ToQuery(), cancellationToken);

        return Ok(page.ToResponse());
    }

    [HttpPut]
    [ProducesResponseType<IReadOnlyList<UpEntityTemplateResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<UpEntityTemplateResponse>>> Update(
        [MinLength(1), MaxLength(MaxUpEntityTemplatesPerRequest)] IReadOnlyList<UpdateUpEntityTemplateRequest> requests,
        CancellationToken cancellationToken)
    {
        if (requests.DistinctBy(request => request.Id).Count() != requests.Count)
        {
            ModelState.AddModelError(string.Empty, "Each UpEntityTemplate can appear only once.");
            return ValidationProblem(ModelState);
        }

        UpdateUpEntityTemplatesResult result = await upEntityTemplateService.UpdateAsync(
            [.. requests.Select(request => request.ToUpEntityTemplate())],
            cancellationToken);

        if (result.MissingIds.Count > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: $"UpEntityTemplates not found: {string.Join(", ", result.MissingIds)}");
        }

        return Ok(result.Updated.ToResponses());
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        [FromQuery, Required, MinLength(1), MaxLength(MaxUpEntityTemplatesPerRequest)] Guid[] ids,
        CancellationToken cancellationToken)
    {
        DeleteUpEntityTemplatesResult result = await upEntityTemplateService.DeleteAsync(
            [.. ids.Distinct()],
            cancellationToken);

        if (result.MissingIds.Count > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: $"UpEntityTemplates not found: {string.Join(", ", result.MissingIds)}");
        }

        if (result.TypesInUse.Count > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: $"UpEntityTemplates are still used by UpEntities: {string.Join(", ", result.TypesInUse)}");
        }

        return NoContent();
    }
}
