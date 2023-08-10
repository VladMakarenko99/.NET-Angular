using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Interfaces;
using API.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/steam-signin";
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.LogoutPath = "/steam-signout";
        })
    .AddSteam(options => {
        options.ClaimsIssuer = "Steam";
    });

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();