using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazingQuiz.Api.Services;

public class AuthService
{
    private readonly QuizContext context;
    private readonly IPasswordHasher<User> passwordHasher;
    private readonly IConfiguration configuration;

    public AuthService(QuizContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        this.context = context;
        this.passwordHasher = passwordHasher;
        this.configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == dto.Username);
        
        if (user == null)
        {
            // invalid username
            return new AuthResponseDto(string.Empty, "Invalid username");
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            // incorrect password
            return new AuthResponseDto(string.Empty, "Incorrect Password");
        }

        // generate jwt token
        var jwt = GenerateJwtToken(user);

        return new AuthResponseDto(jwt);
    }

    private string GenerateJwtToken(User user)
    {
        Claim[] claims = 
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
        ];

        var secretKey = configuration.GetValue<string>("Jwt:Secret");

        var symmetricKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        
        var signingCred = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);
        
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("Jwt:Issuer"),
            audience: configuration.GetValue<string>("Jwt:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpireInMinutes")),
            signingCredentials: signingCred
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return token;
    }
}
