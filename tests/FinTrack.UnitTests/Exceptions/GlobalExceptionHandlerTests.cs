using FinTrack.Api.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace FinTrack.UnitTests.Exceptions;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler = new(NullLogger<GlobalExceptionHandler>.Instance);

    [Fact]
    public async Task TryHandle_WhenValidationExceptionOccurs_ReturnsValidationProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new ValidationException(
            new[]
            {
                new ValidationFailure("Name", "O nome é obrigatório.")
            });

        // Act
        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        var body = await ReadResponse(context);
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Contains("O nome é obrigatório.", body);
        Assert.Contains("A validação da requisição falhou.", body);
    }

    [Fact]
    public async Task TryHandle_WhenEntityDoesNotExist_ReturnsProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            context,
            new KeyNotFoundException("Conta não encontrada."),
            CancellationToken.None);

        // Assert
        var body = await ReadResponse(context);
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Contains("Conta não encontrada.", body);
    }

    [Fact]
    public async Task TryHandle_WhenUnexpectedExceptionOccurs_ReturnsInternalProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            context,
            new InvalidOperationException("Detalhe interno"),
            CancellationToken.None);

        // Assert
        var body = await ReadResponse(context);
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("Ocorreu um erro interno.", body);
        Assert.DoesNotContain("Detalhe interno", body);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadResponse(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
