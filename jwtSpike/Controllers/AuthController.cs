using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace jwtSpike.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly JwtDbContext _jwtDbContext;
    private readonly ILogger _logger;

    public AuthController(ILogger<AuthController> logger, JwtDbContext jwtDbContext, IConfiguration configuration)
    {
        _logger = logger;
        _jwtDbContext = jwtDbContext;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenViewModel tokenViewModel, CancellationToken cancellationToken)
    {
        if (tokenViewModel is null) return BadRequest("Invalid client request.");

        var accessToken = tokenViewModel.AccessToken;
        var refreshToken = tokenViewModel.RefreshToken;

        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null) return BadRequest("Invalid access token or refresh token.");

        var username = principal.Identity.Name;

        var user = await _jwtDbContext.Users.FirstOrDefaultAsync(_ => _.Name == username, cancellationToken);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Invalid access token or refresh token.");

        var newAccessToken = CreateAccessToken(principal.Claims.ToList());
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _jwtDbContext.SaveChangesAsync(cancellationToken);

        return new JsonResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);
        if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private JwtSecurityToken CreateAccessToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            "https://localhost:7205",
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            claims: claims
        );

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}