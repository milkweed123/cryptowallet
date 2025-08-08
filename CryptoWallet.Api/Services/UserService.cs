using AutoMapper;
using CryptoWallet.Api.DAL.Repositories;
using CryptoWallet.Api.Data.DTOs;
using CryptoWallet.Api.Entities;
using CryptoWallet.Api.Models.Exceptions;


namespace CryptoWallet.Api.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto request);
    Task<User> GetUserBalanceAsync(Guid userId);
    Task<ReadUserDto> DepositAsync(Guid userId, double amount);
    Task<ReadUserDto> WithdrawAsync(Guid userId, double amount);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<User> CreateUserAsync(CreateUserDto request)
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            Balance = 0
        };
        if (!await _repository.EmailIsExistsAsync(request.Email))
        {
            return await _repository.CreateUserAsync(user);
        }
        else throw new UserExistsException();
    }

    public async Task<User> GetUserBalanceAsync(Guid userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        return user ?? throw new UserNotFoundException();
    }

    public async Task<ReadUserDto> DepositAsync(Guid userId, double amount)
    {
        var user = await _repository.GetUserByIdAsync(userId) ?? throw new UserNotFoundException();
        user.Balance += amount;
        await _repository.UpdateUserAsync(user);
        return _mapper.Map<ReadUserDto>(user);
    }

    public async Task<ReadUserDto> WithdrawAsync(Guid userId, double amount)
    {
        var user = await _repository.GetUserByIdAsync(userId) ?? throw new UserNotFoundException();
        if (user.Balance < amount) throw new InsufficientFundsException();

        user.Balance -= amount;
        await _repository.UpdateUserAsync(user);
        return _mapper.Map<ReadUserDto>(user);
    }
}
