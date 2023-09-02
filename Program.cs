using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Interfaces;
using API.Repository;
using Microsoft.IdentityModel.Tokens;
using API;
using System.Text;
using Microsoft.Net.Http.Headers;

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

//Authentication and authorization
builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "JWT_OR_COOKIE";
            options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        })
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = "/steam-signin";
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.LogoutPath = "/steam-signout";
        })
        .AddSteam(options =>
        {
            options.ClaimsIssuer = "Steam";
        })
        .AddJwtBearer("Bearer", options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration["Jwt:Key"]!)),
            };
        })
        .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
        {
            options.ForwardDefaultSelector = context =>
        {
            string authorization = context.Request.Headers[HeaderNames.Authorization]!;
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return "Bearer";

            return "Cookies";
        };
        });

builder.Services.AddCors();

//Repositories
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();

//Database context
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Background service
builder.Services.AddHostedService<ExpiredServicesCleanupService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();