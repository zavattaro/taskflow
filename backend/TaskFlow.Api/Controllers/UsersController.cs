using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts;
using TaskFlow.Api.Contracts.Users;
using TaskFlow.Api.Errors;
using TaskFlow.Application.Users.LoginUser;
using TaskFlow.Application.Users.RegisterUser;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly LoginUserUseCase _loginUserUseCase;
    private readonly RegisterUserUseCase _registerUserUseCase;

    public UsersController(LoginUserUseCase loginUserUseCase, RegisterUserUseCase registerUserUseCase)
    {
        _loginUserUseCase = loginUserUseCase;
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ApiErrorResponse(ErrorMessages.EmailRequired));

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiErrorResponse(ErrorMessages.PasswordRequired));

        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await _loginUserUseCase.ExecuteAsync(command);

        if (result is null)
            return Unauthorized(new ApiErrorResponse(ErrorMessages.InvalidCredentials));

        return Ok(new LoginUserResponse(result.Token, result.ExpiresAt));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ApiErrorResponse(ErrorMessages.NameRequired));

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ApiErrorResponse(ErrorMessages.EmailRequired));

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiErrorResponse(ErrorMessages.PasswordRequired));

        var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
        var result = await _registerUserUseCase.ExecuteAsync(command);

        var response = new RegisterUserResponse(result.UserId, result.Name, result.Email);
        return Created($"/api/users/{result.UserId}", response);
    }
}
