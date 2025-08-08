using CryptoWallet.Api.Data.DTOs;
using CryptoWallet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CryptoWallet.Api.Controllers;

[ApiController]
[Route("api/users")]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto">User creation data (email)</param>
    /// <returns>User data including balance</returns>
    /// <response code="200">User successfully created</response>
    /// <response code="409">User with this email already exists</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return Ok(user);
    }

    /// <summary>
    /// Retrieves the balance of a user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User ID and current balance</returns>
    /// <response code="200">Balance retrieved successfully</response>
    /// <response code="404">User not found</response>
    [HttpGet("{userId}/balance")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var userBalance = await _userService.GetUserBalanceAsync(userId);

        return Ok(userBalance);
    }

    /// <summary>
    /// Deposits an amount into the user's account.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dto">Amount to deposit</param>
    /// <returns>User ID and new balance</returns>
    /// <response code="200">Deposit successful</response>
    /// <response code="404">User not found</response>
    [HttpPost("{userId}/deposit")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deposit(Guid userId, [FromBody] DepositDto dto)
    {
        var result = await _userService.DepositAsync(userId, dto.Amount);
        return Ok(result);
    }

    /// <summary>
    /// Withdraws an amount from the user's account.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dto">Amount to withdraw</param>
    /// <returns>User ID and new balance</returns>
    /// <response code="200">Withdrawal successful</response>
    /// <response code="409">Insufficient funds</response>
    /// <response code="404">User not found</response>
    [HttpPost("{userId}/withdraw")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(Guid userId, [FromBody] WithdrawRequest dto)
    {
        var result = await _userService.WithdrawAsync(userId, dto.Amount);
        return Ok(result);
    }
}
