using FinTrack.Application.Features.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{

    [HttpPost(Name = "CreateCategory")]
    [ProducesResponseType(typeof(CreateCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);
        return CreatedAtRoute("GetByIdCategory", new { id = response.Id }, response);
    }

    [HttpGet("{id}", Name = "GetByIdCategory")]
    [ProducesResponseType(typeof(GetByIdCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetByIdCategoryQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet(Name = "GetAllCategories")]
    [ProducesResponseType(typeof(GetAllCategoriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateCategory")]
    [ProducesResponseType(typeof(UpdateCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new UpdateCategoryCommand(id, request.Name, request.Type, request.Color), cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteCategory")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }

}
