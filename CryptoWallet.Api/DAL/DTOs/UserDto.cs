using System.ComponentModel.DataAnnotations;

namespace CryptoWallet.Api.Data.DTOs;

public record CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}

public record ReadUserDto
{
    public Guid Guid { get; init; }

    public string Email { get; init; } = string.Empty;
}
