using API.Interfaces;
using API.Models;
using System.Text.Json;


namespace API;

public class ExpiredServicesCleanupService : BackgroundService
{
    private readonly ILogger<ExpiredServicesCleanupService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ExpiredServicesCleanupService(
         ILogger<ExpiredServicesCleanupService> logger,
         IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var _serviceRepository = scope.ServiceProvider.GetRequiredService<IServiceRepository>();
            _logger.LogInformation("Checking for expired services...");

            var users = await _userRepository.GetUsersAsync();

            foreach (var user in users)
            {
                if (user.BoughtServicesJson != null)
                {
                    var expiredServices = JsonSerializer.Deserialize<List<Service>>(user!.BoughtServicesJson!)
                    ?.Where(service => DateTime.Parse(service.ExpireDate!) <= DateTime.Now)
                    .ToList();

                    await _serviceRepository.RemoveExpiredServicesAsync(expiredServices!, user.SteamId);
                }
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}