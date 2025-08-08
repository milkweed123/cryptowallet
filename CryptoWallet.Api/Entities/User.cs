using System.ComponentModel.DataAnnotations;

namespace CryptoWallet.Api.Entities;

public class User
{

    public Guid UserId { get; set; }


    public string Email { get; set; } = string.Empty;


    public double Balance { get; set; } = 0;
}
