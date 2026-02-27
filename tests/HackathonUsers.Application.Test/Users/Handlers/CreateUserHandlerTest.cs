using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using HackathonUsers.Application.Users.Commands;
using HackathonUsers.Application.Users.Handlers;
using HackathonUsers.Domain.Models;

namespace HackathonUsers.Application.Test.Users.Handlers;

public class CreateUserHandlerTest
{
    private static readonly IPasswordValidator<User>[] PasswordValidators = [new PasswordValidator<User>()];
    private static readonly Mock<IUserEmailStore<User>> Store = new();
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    private static readonly IdentityOptions IdentityOptions = new() { Password = { RequireDigit = true, RequireLowercase = false, RequireNonAlphanumeric = true, RequireUppercase = false, RequiredLength = 8, RequiredUniqueChars = 0 }};
    private static readonly IOptions<IdentityOptions> Options = Microsoft.Extensions.Options.Options.Create(IdentityOptions);
    
    private readonly Mock<UserManager<User>> _manager = new(Store.Object, Options, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<CreateUserHandler>> _logger = new();
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTest()
        => _handler = new CreateUserHandler(_manager.Object, Store.Object, _mapper.Object, _logger.Object);
    
    [Fact]
    public async Task Handle_WhenInvalidEmail_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "testeexemplo.com", Name = "Teste", Password = "Passw0rd@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Email 'testeexemplo.com' is invalid.", result.Exception.Message);
    }

    [Fact]
    public async Task Handle_WhenValidEmail_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Passw0rd@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_WhenPasswordLessThanEightCharacters_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Passw0@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must be at least 8 characters.", result.Exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenPasswordWithoutDigit_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Password@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must have at least one digit ('0'-'9').", result.Exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_WhenPasswordWithoutLetter_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "12345678" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must have at least one non alphanumeric character.", result.Exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_WhenValidPassword_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Passw0rd@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
}