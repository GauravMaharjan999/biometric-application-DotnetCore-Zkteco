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
            ScheduleNextExecution();
            return Task.CompletedTask;
		}

        private void ScheduleNextExecution()
         {
            try
            {
                var fetchIntervalTimeInMinutes = _configuration.GetSection("AppCustomSettings").GetSection("FetchTimeIntervalInMinutes").Value;
                var fetchTimeTriggerMode = _configuration.GetSection("AppCustomSettings").GetSection("FetchTimeTriggerMode").Value;
                if (int.Parse(fetchTimeTriggerMode) == 1) //FetchTimeTriggerMode 1 Means the attendanceFetchService run everly minutely 
                {
                    _timer = new Timer(ScheduleAsyncAutoPushDataToMainServer, null, TimeSpan.Zero, TimeSpan.FromMinutes(Int32.Parse(fetchIntervalTimeInMinutes)));

                }
                else //FetchTimeTriggerMode 2 Means the attendanceFetchService run everly Hourly like 10:15,11:15,12:15.............
                {
                    DateTime now = DateTime.Now;
                    DateTime nextRunTime = CalculateNextRunTimeHourly(now, int.Parse(fetchIntervalTimeInMinutes));

                    TimeSpan timeUntilNextRun = nextRunTime - now;

                    if (timeUntilNextRun < TimeSpan.Zero)
                    {
                        timeUntilNextRun = TimeSpan.Zero - timeUntilNextRun; // Get the absolute value
                    }
                    // Calculate the period until the next occurrence of 10 minutes past the hour
                    TimeSpan period = TimeSpan.FromHours(1);

                    // If the time until the next run exceeds the period, adjust it
                    if (timeUntilNextRun > period)
                    {
                        timeUntilNextRun = period;
                    }
                    _timer = new Timer(_ =>
                    {
                        ScheduleAsyncAutoPushDataToMainServer(null);
                        ScheduleNextExecution();
                    }, null, timeUntilNextRun, period);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
          
        }



        private DateTime CalculateNextRunTimeHourly(DateTime currentDateTime, int targetMinute) //This method calculate every hour to trigger
        {
            // Calculate the next desired execution time (e.g., 10:15, 11:15, etc.)
            int currentHour = currentDateTime.Hour;
            int currentMinute = currentDateTime.Minute;

            // Add hour , Example if current time is 10:40 and target min is 15 then this will make the targethour 11:15
            if (targetMinute<currentMinute) 
            {
                currentHour = (currentHour + 1) % 24;
            }

            // Set the target time
            return new DateTime(
                currentDateTime.Year,
                currentDateTime.Month,
                currentDateTime.Day,
                currentHour,
                targetMinute,
                0);
        }


        private void ScheduleAsyncAutoPushDataToMainServer(object state)
		{
			// Your recurring job logic goes here
			 _services.ScheduleAsyncAutoPushDataToMainServer();
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
