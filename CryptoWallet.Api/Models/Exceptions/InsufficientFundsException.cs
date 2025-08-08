namespace CryptoWallet.Api.Models.Exceptions;

public class InsufficientFundsException : BaseException
{
    public InsufficientFundsException(string message =  "Insufficient Funds", int statusCode = StatusCodes.Status409Conflict) 
        : base(message, statusCode: statusCode)
    {
    }
}
