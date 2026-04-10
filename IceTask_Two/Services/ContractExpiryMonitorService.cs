using ContractCentral.Models;
using IceTask_Two.Models;
using Microsoft.EntityFrameworkCore;

namespace IceTask_Two.Services
{
    /// <summary>
    /// Runs in the background and marks Active / OnHold contracts as Expired when EndDate is in the past.
    /// </summary>
    public class ContractExpiryMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ContractExpiryMonitorService> _logger;

        // How often to check (simple demo value — you could change this to hours for a real system)
        private static readonly TimeSpan CheckEvery = TimeSpan.FromMinutes(1);

        public ContractExpiryMonitorService(
            IServiceScopeFactory scopeFactory,
            ILogger<ContractExpiryMonitorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait a few seconds on startup so the web app finishes booting first
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunExpiryCheckOnce(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Contract expiry check crashed (will try again later).");
                }

                await Task.Delay(CheckEvery, stoppingToken);
            }
        }

        private async Task RunExpiryCheckOnce(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var today = DateTime.Today;

            // Only auto-expire contracts that are still "in progress" types
            var list = await db.Contract_Info
                .Where(c =>
                    (c.Status == ContractStatus.Active || c.Status == ContractStatus.OnHold)
                    && c.EndDate.Date < today)
                .ToListAsync(stoppingToken);

            if (list.Count == 0)
            {
                return;
            }

            foreach (var contract in list)
            {
                contract.Status = ContractStatus.Expired;
            }

            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation(
                "Expiry monitor: set {Count} contract(s) to Expired (end date before {Today}).",
                list.Count,
                today.ToString("yyyy-MM-dd"));
        }
    }
}
