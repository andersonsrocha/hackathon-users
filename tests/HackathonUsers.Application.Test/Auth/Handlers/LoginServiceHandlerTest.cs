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

public class LoginServiceHandlerTest
{
    private readonly Mock<IServiceClientValidation> _clientValidator = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<ILogger<LoginServiceHandler>> _logger = new();
    private readonly LoginServiceHandler _handler;

    public LoginServiceHandlerTest()
        => _handler = new LoginServiceHandler(_jwtService.Object, _clientValidator.Object, _logger.Object);
    
    [Fact]
    public async Task Handle_WhenClientIdIsEmpty_ShouldReturnError()
    {
        // Arrange
        var request = new LoginServiceRequest { ClientId = Guid.Empty, ClientSecret = "SenhaExtremamenteForte123" };
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("ClientId is required", result.Exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenClientSecretIsEmpty_ShouldReturnError()
    {
        // Arrange
        var request = new LoginServiceRequest { ClientId = Guid.NewGuid(), ClientSecret = "" };
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("ClientSecret is required", result.Exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenInvalidClientIdOrClientSecret_ShouldReturnError()
    {
        // Arrange
        var request = new LoginServiceRequest { ClientId = Guid.NewGuid(), ClientSecret = "SenhaExtremamenteForte123" };
        _clientValidator.Setup(x => x.Validate(request.ClientId, request.ClientSecret)).Returns(false);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid client credentials", result.Exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenValidClientIdOrClientSecret_ShouldReturnSuccess()
    {
        // Arrange
        var request = new LoginServiceRequest { ClientId = Guid.NewGuid(), ClientSecret = "SenhaExtremamenteForte123" };
        _clientValidator.Setup(x => x.Validate(request.ClientId, request.ClientSecret)).Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task LoginJwtGenerate_WhenValidEmailAndPassword_ShouldReturnSuccess()
    {
        // Arrange
        var request = new LoginServiceRequest { ClientId = Guid.NewGuid(), ClientSecret = "SenhaExtremamenteForte123" };
        _clientValidator.Setup(x => x.Validate(request.ClientId, request.ClientSecret)).Returns(true);
        _jwtService.Setup(x => x.Generate(request.ClientId)).Returns("jwt-token");
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value.Token);
    }
}