using System;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using AttendanceFetch.Helpers.Schedular;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace BiometricDataFetchAPI
{
	public class RecurringJobService : BackgroundService
	{
		private Timer _timer;
		private readonly IJobSchedular _services;
		private readonly IConfiguration _configuration;

		public RecurringJobService(IJobSchedular services, IConfiguration configuration)
		{
			_services = services;
			_configuration = configuration;
		}
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var fetchIntervalTimeInMinutes = _configuration.GetSection("AppCustomSettings").GetSection("FetchTimeIntervalInMinutes").Value;
			_timer = new Timer(ScheduleAsyncAutoPushDataToMainServer, null, TimeSpan.Zero, TimeSpan.FromMinutes(Int32.Parse(fetchIntervalTimeInMinutes)));

			return Task.CompletedTask;
		}

		private void ScheduleAsyncAutoPushDataToMainServer(object state)
		{
			// Your recurring job logic goes here
			_services.ScheduleAsyncAutoPushDataToMainServer();
		}

		private void ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning(object state)
		{
			// Your recurring job logic goes here
			_services.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning();
		}

		private void ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening(object state)
		{
			// Your recurring job logic goes here
			_services.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening();
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
