using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HackathonUsers.Domain.Models;

namespace HackathonUsers.Security;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string Generate(User user, IEnumerable<string> roles)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(user, roles),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(5),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
        };
        
        var token = handler.CreateToken(tokenDescriptor);
        
        return handler.WriteToken(token);
    }
    
    private static ClaimsIdentity GenerateClaims(User user, IEnumerable<string> roles)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Name));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
        foreach (var role in roles) 
            ci.AddClaim(new Claim(ClaimTypes.Role, role));

        return ci;
    }
}