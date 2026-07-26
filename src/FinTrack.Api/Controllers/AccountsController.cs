using FinTrack.Application.Features.Accounts;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ISender _sender;

    public AccountsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return CreatedAtRoute("GetAccountById", new { id = response.Id }, response);
    }

    [HttpGet("{id}", Name = "GetAccountById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok();
    }

    [HttpGet]
    public IActionResult Teste()
    {
        return Ok("O Controller está vivo!");
    }
}
