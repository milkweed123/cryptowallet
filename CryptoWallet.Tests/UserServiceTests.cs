using AutoMapper;
using CryptoWallet.Api.DAL.Repositories;
using CryptoWallet.Api.Data.DTOs;
using CryptoWallet.Api.Entities;
using CryptoWallet.Api.Models.Exceptions;
using CryptoWallet.Api.Services;
using Moq;


namespace CryptoWallet.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mapper
            .Setup(m => m.Map<ReadUserDto>(It.IsAny<User>()))
            .Returns((User u) => new ReadUserDto { UserId = u.UserId, Email = u.Email });

        _service = new UserService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreate_WhenEmailNotExists()
    {
        var request = new CreateUserDto { Email = "new@ex.com" };
        _repo.Setup(r => r.EmailIsExistsAsync(request.Email)).ReturnsAsync(false);

        User? captured = null;
        var persisted = new User { UserId = Guid.NewGuid(), Email = request.Email, Balance = 0 };
        _repo.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
             .Callback<User>(u => captured = u)
             .ReturnsAsync(persisted);

        var created = await _service.CreateUserAsync(request);

        Assert.NotNull(captured);
        Assert.Equal(request.Email, captured!.Email);
        Assert.Equal(0, captured.Balance);
        Assert.NotEqual(Guid.Empty, captured.UserId);

        Assert.Same(persisted, created);

        _repo.Verify(r => r.EmailIsExistsAsync(request.Email), Times.Once);
        _repo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenEmailExists()
    {
        var request = new CreateUserDto { Email = "dup@ex.com" };
        _repo.Setup(r => r.EmailIsExistsAsync(request.Email)).ReturnsAsync(true);

        await Assert.ThrowsAsync<UserExistsException>(() => _service.CreateUserAsync(request));

        _repo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetUserBalanceAsync_ShouldReturnUser_WhenFound()
    {
        var id = Guid.NewGuid();
        var user = new User { UserId = id, Email = "u@ex.com", Balance = 123.45 };
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync(user);

        var result = await _service.GetUserBalanceAsync(id);

        Assert.Same(user, result);
        _repo.Verify(r => r.GetUserByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetUserBalanceAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(() => _service.GetUserBalanceAsync(id));
    }

    [Fact]
    public async Task DepositAsync_ShouldIncreaseBalance_AndReturnDto()
    {
        var id = Guid.NewGuid();
        var user = new User { UserId = id, Email = "u@ex.com", Balance = 10.0 };
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync(user);
        _repo.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var dto = await _service.DepositAsync(id, 90.0);

        Assert.Equal(id, dto.UserId);
        Assert.Equal("u@ex.com", dto.Email);
        Assert.Equal(100.0, user.Balance, 5);

        _repo.Verify(r => r.UpdateUserAsync(It.Is<User>(u => u.UserId == id && Math.Abs(u.Balance - 100.0) < 1e-6)), Times.Once);
        _mapper.Verify(m => m.Map<ReadUserDto>(user), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrow_WhenUserNotFound()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(() => _service.DepositAsync(id, 10.0));
        _repo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldDecreaseBalance_AndReturnDto()
    {
        var id = Guid.NewGuid();
        var user = new User { UserId = id, Email = "u@ex.com", Balance = 150.0 };
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync(user);
        _repo.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var dto = await _service.WithdrawAsync(id, 50.0);

        Assert.Equal(id, dto.UserId);
        Assert.Equal(100.0, user.Balance, 5);

        _repo.Verify(r => r.UpdateUserAsync(It.Is<User>(u => u.UserId == id && Math.Abs(u.Balance - 100.0) < 1e-6)), Times.Once);
        _mapper.Verify(m => m.Map<ReadUserDto>(user), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrow_WhenInsufficientFunds()
    {
        var id = Guid.NewGuid();
        var user = new User { UserId = id, Email = "u@ex.com", Balance = 20.0 };
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync(user);

        await Assert.ThrowsAsync<InsufficientFundsException>(() => _service.WithdrawAsync(id, 100.0));


        Assert.Equal(20.0, user.Balance, 5);
        _repo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        _mapper.Verify(m => m.Map<ReadUserDto>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrow_WhenUserNotFound()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(() => _service.WithdrawAsync(id, 10.0));
        _repo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}
