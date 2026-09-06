using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/accounts/{accountId:guid}/[controller]")]
public class TransactionsController(ISender sender) : ControllerBase
{

    [HttpPost(Name = "CreateTransaction")]
    [ProducesResponseType(typeof(CreateTransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(Guid accountId, [FromBody] CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new CreateTransactionCommand(
            accountId,
            request.CategoryId,
            request.Description,
            request.Amount,
            request.Type,
            request.Date,
            request.Status
        ), cancellationToken);

        return CreatedAtRoute("GetByIdTransaction", new { id = response.Id, accountId = response.AccountId }, response);
    }

    [HttpGet("{id:guid}", Name = "GetByIdTransaction")]
    [ProducesResponseType(typeof(GetByIdTransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] GetByIdTransactionQuery request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("api/accounts/{accountId:guid?}/[controller]", Name = "GetAllTransactions")]
    [ProducesResponseType(typeof(GetAllTransactionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromRoute] Guid? accountId, [FromQuery] TransactionType? type, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetAllTransactionsQuery(accountId, type), cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}", Name = "UpdateTransaction")]
    [ProducesResponseType(typeof(UpdateTransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid id, Guid accountId, [FromBody] UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new UpdateTransactionCommand(
            id,
            accountId,
            request.Description,
            request.Amount,
            request.Type,
            request.Date,
            request.Status
        ), cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{id:guid}", Name = "DeleteTransaction")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return NoContent();
    }
}
