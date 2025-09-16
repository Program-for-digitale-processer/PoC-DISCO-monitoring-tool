using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PoC_DISCO_Backend;
using PoC_DISCO_Backend.Data;
using PoC_DISCO_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<DiscoToolContext>(options =>
{
    var folder = Environment.SpecialFolder.LocalApplicationData;
    var path = Environment.GetFolderPath(folder);
    var dbPath = Path.Join(path, "DiscoTool.db");
    
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtConfig = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtConfig["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DiscoToolContext>();
    await context.Database.EnsureCreatedAsync();

    if (!context.Users.Any())
    {
        var rawUsers = new[]
        {
            new { Username = "user1", Password = "Passw0rd!1" },
            new { Username = "user2", Password = "Passw0rd!2" },
            new { Username = "user3", Password = "Passw0rd!3" },
        };

        var usersToSeed = new List<User>();

        foreach (var rawUser in rawUsers)
        {
            var user = new User
            {
                UserName = rawUser.Username,
            };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawUser.Password);
            usersToSeed.Add(user);
        }

        context.Users.AddRange(usersToSeed);
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

