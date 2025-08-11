using CryptoWallet.Api.Controllers;
using CryptoWallet.Api.Data.DTOs;
using CryptoWallet.Api.Entities;
using CryptoWallet.Api.Models.Exceptions;
using CryptoWallet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CryptoWallet.Tests;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly Mock<IUserService> _mockService;

    public UsersControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UsersController(_mockService.Object);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnOk_WithUser()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "test@example.com" };
        var expectedUser = new User
        {
            UserId = Guid.NewGuid(),
            Email = createDto.Email,
            Balance = 0
        };

        _mockService
            .Setup(s => s.CreateUserAsync(createDto))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.CreateUser(createDto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(ok.Value);
        Assert.Equal(expectedUser.Email, returnedUser.Email);
        Assert.Equal(expectedUser.UserId, returnedUser.UserId);
        Assert.Equal(expectedUser.Balance, returnedUser.Balance);
    }


    [Fact]
    public async Task GetBalance_ShouldReturnOk_WhenUserFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            UserId = userId,
            Email = "user@example.com",
            Balance = 123.45
        };

        _mockService
            .Setup(s => s.GetUserBalanceAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetBalance(userId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = ok.Value!;
        var type = response.GetType();

        Assert.Equal(userId, (Guid)type.GetProperty("UserId")!.GetValue(response)!);
        Assert.Equal(user.Balance, (double)type.GetProperty("Balance")!.GetValue(response)!);
    }

    [Fact]
    public async Task GetBalance_ShouldThrow_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();

        _mockService
            .Setup(s => s.GetUserBalanceAsync(userId))
            .ThrowsAsync(new UserNotFoundException());

        await Assert.ThrowsAsync<UserNotFoundException>(() => _controller.GetBalance(userId));
    }



    [Fact]
    public async Task Deposit_ShouldReturnOk_WithReadUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var depositAmount = 100.0;
        var expectedDto = new ReadUserDto
        {
            UserId = userId,
            Email = "user@example.com"
        };

        _mockService
            .Setup(s => s.DepositAsync(userId, depositAmount))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.Deposit(userId, new DepositDto { Amount = depositAmount });

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<ReadUserDto>(ok.Value);
        Assert.Equal(expectedDto.UserId, returnedDto.UserId);
    }

    [Fact]
    public async Task Withdraw_ShouldReturnOk_WithReadUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var withdrawAmount = 50.0;
        var expectedDto = new ReadUserDto
        {
            UserId = userId,
            Email = "user@example.com"
        };

        _mockService
            .Setup(s => s.WithdrawAsync(userId, withdrawAmount))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.Withdraw(userId, new WithdrawDto { Amount = withdrawAmount });

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<ReadUserDto>(ok.Value);
        Assert.Equal(expectedDto.UserId, returnedDto.UserId);
    }
}
