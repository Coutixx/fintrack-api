using FinTrack.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(ISender sender) : ControllerBase
{

    [HttpPost(Name = "CreateAccount")]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);
        return CreatedAtRoute("GetByIdAccount", new { id = response.Id }, response);
    }

    [HttpGet("{id}", Name = "GetByIdAccount")]
    [ProducesResponseType(typeof(GetByIdAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetByIdAccountQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet(Name = "GetAllAccounts")]
    [ProducesResponseType(typeof(GetAllAccountsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetAllAccountsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateAccount")]
    [ProducesResponseType(typeof(UpdateAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new UpdateAccountCommand(id, request.Name, request.Type), cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteAccount")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteAccountCommand(id), cancellationToken);
        return NoContent();
    }

}
