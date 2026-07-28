using FinTrack.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(ISender sender) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);

        return CreatedAtRoute("GetByIdAccount", new { id = response.Id }, response);
    }

    [HttpGet(Name = "GetAllAccounts")]
    [ProducesResponseType(typeof(GetAllAccountsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetByIdAccount")]
    [ProducesResponseType(typeof(GetByIdAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdAccountQuery(id);
        var response = await sender.Send(query, cancellationToken);
        return Ok(response);
    }
}
