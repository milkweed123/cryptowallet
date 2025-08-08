namespace CryptoWallet.Api.Models.Exceptions;

public class UserNotFoundException : BaseException
{
    public UserNotFoundException(string message = "User not found", int statusCode = StatusCodes.Status404NotFound) : base(message, statusCode: statusCode)
    {
    }
}
