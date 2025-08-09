using System.ComponentModel.DataAnnotations;

namespace CryptoWallet.Api.Data.DTOs;

public record WithdrawDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public double Amount { get; set; }
}