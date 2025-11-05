using JwtAuth.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JwtAuth.Interfaces;
using JwtAuth.Services;
using JwtAuth.Repositories;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var authGroup = app.MapGroup("/auth");

authGroup.MapPost("/register", async (IUserService userService, string username, string password) =>
{
    var existingUser = await userService.GetByUsernameAsync(username);
    if (existingUser != null)
        return Results.BadRequest(new { Message = "User already exists" });

    var user = await userService.RegisterAsync(username, password);
    var token = userService.GenerateJwtToken(user);
    return Results.Ok(new { Token = token });
});

authGroup.MapPost("/login", async (IUserService userService, string username, string password) =>
{
    var token = await userService.LoginAsync(username, password);
    return token is null
        ? Results.Unauthorized()
        : Results.Ok(new { Token = token });
});

authGroup.MapGet("/verify", [Authorize] async (HttpContext httpContext) =>
{
    var userClaims = httpContext.User;
    if (userClaims?.Identity?.IsAuthenticated == true)
    {
        var username = userClaims.Identity.Name;
        return Results.Ok(new { Message = "Token is valid", UserName = username });
    }
    return Results.Unauthorized();
});

authGroup.MapGet("/me", async (IUserService userService, HttpContext http) =>
{
    var username = http.User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return Results.Unauthorized();

    var user = await userService.GetByUsernameAsync(username);
    return user is null
        ? Results.NotFound(new { Message = "User not found" })
        : Results.Ok(new { UserName = user.UserName });
}).RequireAuthorization();

app.Run();
