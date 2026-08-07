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
    public async Task<IActionResult> GetById([FromQuery] GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);
        return Ok(response);
    }

}
