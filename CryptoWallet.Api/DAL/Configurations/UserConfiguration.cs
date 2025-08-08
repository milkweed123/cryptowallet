using CryptoWallet.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWallet.Api.Dal.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId).HasColumnName("userId");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(u => u.Balance)
            .IsRequired()
            .HasColumnName("balance")
            .HasDefaultValue(0)
            .HasPrecision(18, 2); 

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
