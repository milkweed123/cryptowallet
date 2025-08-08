namespace CryptoWallet.Api.Models.Exceptions;

public class UserExistsException : BaseException
{
    public UserExistsException(string message = "User already exists", int statusCode = StatusCodes.Status409Conflict) : base(message, statusCode: statusCode)
    {
    }
}
