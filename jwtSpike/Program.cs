using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwtSpike;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var a = new JwtSecurityTokenHandler().WriteToken(GetToken(builder.Configuration["JWT:Secret"], "test"));


builder.Services.AddAuthorization();

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
        ValidAudience = "test",
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https://localhost:7205",
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<JwtDbContext>(options => options.UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=jwt;Integrated Security=True;"));

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static JwtSecurityToken GetToken(string secret, string username)
{
    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    var token = new JwtSecurityToken(
        issuer: "https://localhost:7205",
        audience: "test",
        expires: DateTime.Now.AddMinutes(1),
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
        claims: new []{ new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), new Claim(JwtRegisteredClaimNames.Name, username) }
    );

    return token;
}
