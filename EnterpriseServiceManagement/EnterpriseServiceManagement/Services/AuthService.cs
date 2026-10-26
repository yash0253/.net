using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Identity;
using EnterpriseServiceManagement.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace EnterpriseServiceManagement.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _usermanager;
    private readonly IConfiguration _configuration;



    public AuthService(
     UserManager<AppUser> userManager,
     IConfiguration configuration)
    {
        _usermanager = userManager;
        _configuration = configuration;
    }


    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim();

        var existingUser = await _usermanager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = dto.FullName.Trim(),
            IsActive = true
        };

        // Create login account
        var result = await _usermanager.CreateAsync(
            user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        // Assign default Employee role
        var roleResult = await _usermanager.AddToRoleAsync(
            user, "Employee");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(e => e.Description));

            await _usermanager.DeleteAsync(user);

            throw new InvalidOperationException(errors);
        }

        return "Registration successful. Employee role assigned.";
    }
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim();

        var user = await _usermanager.FindByEmailAsync(email);

        if(user ==null || !await _usermanager.CheckPasswordAsync(
            user, dto.Password))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your account is inactive.");

        }

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT issuer is missing.");
        var issuer = _configuration["Jwt:Issuer"]
           ?? throw new InvalidOperationException(
               "JWT issuer is missing.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is missing.");

        var expiryMinutes = int.TryParse(
            _configuration["Jwt:ExpiryMinutes"],
            out var minutes)
                ? minutes
                : 60;

        var now = DateTime.UtcNow;
        var expiresAt =now.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub,user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email,user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email!)
            //This code is from your .NET JWT authentication logic. It creates three claims that are stored inside the JWT (JSON Web Token) when a user logs in.
        };

        var roles = await _usermanager.GetRolesAsync(user);

        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials =new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);

        var token =new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
           .WriteToken(token);

        // 12. Return the login response
        return new LoginResponseDto
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresAt = expiresAt
        };

    }
}

//Understand the important parts
//- UserManager<AppUser> is provided by ASP.NET Core Identity and manages user creation.
//- FindByEmailAsync() checks whether an account already uses the email.
//- CreateAsync(user, dto.Password) creates the account and hashes the password using Identity's password hasher.
//- result.Succeeded tells us whether the account was created successfully.