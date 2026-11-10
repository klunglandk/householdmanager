using HouseholdManager.Services;
using Microsoft.Extensions.Options;

namespace HouseholdManager.Infrastructure
{
    public class RecurrenceGenerationBackgroundService : BackgroundService
    {
        public RecurrenceGenerationBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<RecurrenceGenerationOptions> options,
            TimeProvider timeProvider,
            ILogger<RecurrenceGenerationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _timeZone = _options.GetTimeZone();
            _timeProvider = timeProvider;
            _logger = logger;
        }

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RecurrenceGenerationOptions _options;
        private readonly TimeZoneInfo _timeZone;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<RecurrenceGenerationBackgroundService> _logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                GenerateInstances();

                try
                {
                    await Task.Delay(GetDelayUntilNextRun(), _timeProvider, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void GenerateInstances()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var generator = scope.ServiceProvider.GetRequiredService<IChoreInstanceGenerator>();

                var today = DateOnly.FromDateTime(GetLocalNow().DateTime);
                var createdCount = generator.GenerateInstances(today, today.AddDays(_options.HorizonDays));

                _logger.LogInformation("Chore instance generation created {CreatedCount} instances", createdCount);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Chore instance generation failed");
            }
        }

        private TimeSpan GetDelayUntilNextRun()
        {
            var now = GetLocalNow().DateTime;
            var nextRun = now.Date + _options.DailyRunTime.ToTimeSpan();
            if (nextRun <= now) nextRun = nextRun.AddDays(1);
            return nextRun - now;
        }

        private DateTimeOffset GetLocalNow()
        {
            return TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), _timeZone);
        }
    }
}
