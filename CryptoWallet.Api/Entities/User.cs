using System.ComponentModel.DataAnnotations;

namespace CryptoWallet.Api.Entities;

public class User
{
    [Key]
    public Guid UserId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public double Balance { get; set; } = 0;
}
