using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Interfaces;
using API.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/steam-signin";
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.LogoutPath = "/steam-signout";
        })
    .AddSteam(options =>
    {
        options.ClaimsIssuer = "Steam";
    });
builder.Services.AddCors();


builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();


builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//app.UseMvc();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();