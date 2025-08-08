namespace CryptoWallet.Api.Models.Exceptions;

public class RestApiError
{

    public required string Title { get; init; }  
    public required string Message { get; init; }
}