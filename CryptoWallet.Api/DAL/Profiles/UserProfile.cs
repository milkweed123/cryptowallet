using AutoMapper;
using CryptoWallet.Api.Data.DTOs;
using CryptoWallet.Api.Entities;
namespace CryptoWallet.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();

        CreateMap<User, ReadUserDto>();
    }
}
