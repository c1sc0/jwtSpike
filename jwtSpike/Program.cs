using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//var a = new JwtSecurityTokenHandler().WriteToken(GetToken(builder.Configuration["JWT:Secret"]));



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidAudience = "https://localhost:7205",
        ValidateAudience = true,
        ValidIssuer = "https://localhost:7205",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static JwtSecurityToken GetToken(string secret)
{
    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    var token = new JwtSecurityToken(
        issuer: "https://localhost:7205",
        audience: "https://localhost:7205",
        expires: DateTime.Now.AddHours(3),
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
        claims: new []{ new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) }
    );

    return token;
}
