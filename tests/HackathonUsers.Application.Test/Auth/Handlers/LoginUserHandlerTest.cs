using HackathonUsers.Application.Auth.Commands;
using HackathonUsers.Application.Auth.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using HackathonUsers.Application.Users.Commands;
using HackathonUsers.Application.Users.Handlers;
using HackathonUsers.Domain.Interfaces;
using HackathonUsers.Domain.Models;
using HackathonUsers.Security.Interfaces;

namespace HackathonUsers.Application.Test.Auth.Handlers;

public class LoginUserHandlerTest
{
    private static readonly IPasswordValidator<User>[] PasswordValidators = [new PasswordValidator<User>()];
    private static readonly Mock<IUserEmailStore<User>> Store = new();
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    private static readonly Mock<UserManager<User>> Manager = new(Store.Object, null!, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private static readonly Mock<IHttpContextAccessor> ContextAccessor = new();
    private static readonly Mock<IUserClaimsPrincipalFactory<User>> ClaimsFactory = new();
    private static readonly IdentityOptions IdentityOptions = new() { Password = { RequireDigit = true, RequireLowercase = false, RequireNonAlphanumeric = true, RequireUppercase = false, RequiredLength = 8, RequiredUniqueChars = 0 }};
    private static readonly IOptions<IdentityOptions> Options = Microsoft.Extensions.Options.Options.Create(IdentityOptions);
    
    private readonly Mock<SignInManager<User>> _signInManager = new(Manager.Object, ContextAccessor.Object, ClaimsFactory.Object, null!, null!, null!, null!);
    private readonly Mock<UserManager<User>> _manager = new(Store.Object, Options, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<ILogger<LoginUserHandler>> _logger = new();
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTest()
        => _handler = new LoginUserHandler(_signInManager.Object, _jwtService.Object, _logger.Object);
    
    [Fact]
    public async Task Handle_WhenNotFoundEmail_ShouldReturnError()
    {
        // Arrange
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(null));
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var user = new User();
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Failed);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User or password is incorrect", result.Exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenValidEmailAndPassword_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User();
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Success);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task LoginJwtGenerate_WhenValidEmailAndPassword_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User();
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Success);
        _jwtService.Setup(x => x.Generate(It.IsAny<User>(), It.IsAny<IEnumerable<string>>())).Returns("jwt-token");
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value.Token);
    }
}