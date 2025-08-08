namespace CryptoWallet.Api.Models.Exceptions;

public abstract class BaseException : ApplicationException
{
    public string Title { get; }

    public int StatusCode { get; }

    protected BaseException(string message, string title = "Error", int statusCode = 500)
        : base(message)
    {
        Title = title;
        StatusCode = statusCode;
    }
}