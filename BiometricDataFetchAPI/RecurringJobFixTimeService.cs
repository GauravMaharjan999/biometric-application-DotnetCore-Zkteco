using System;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using AttendanceFetch.Helpers.Schedular;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace BiometricDataFetchAPI
{
	public class RecurringJobFixTimeService : BackgroundService
	{
		private Timer _timer;
		private readonly IJobSchedular _services;

		public RecurringJobFixTimeService(IJobSchedular services)
		{
			_services = services;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//_timer = new Timer(ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

			//return Task.CompletedTask;
			await ScheduleNextExecution();
		}
		private async Task ScheduleNextExecution()
		{
			var now = DateTime.Now;
			DateTime next7AM = new DateTime(now.Year, now.Month, now.Day, 15, 47, 0);
			DateTime next8PM = new DateTime(now.Year, now.Month, now.Day, 12, 50, 0);

			if (now > next7AM)
			{
				next7AM = next7AM.AddDays(1);
			}

			if (now > next8PM)
			{
				next8PM = next8PM.AddDays(1);
			}

			TimeSpan delayUntil7AM = next7AM - DateTime.Now;
			TimeSpan delayUntil8PM = next8PM - DateTime.Now;

			_timer = new Timer(async (_) =>
			{
				await ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening(null);
				await ScheduleNextExecution();
			}, null, delayUntil7AM, delayUntil8PM);
		}

		private async Task ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening(object state)
		{
			// Your recurring job logic goes here
			await _services.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening();
		}

		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return base.StopAsync(cancellationToken);
		}

		public override void Dispose()
		{
			_timer?.Dispose();
			base.Dispose();
		}
	}

}
