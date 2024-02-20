 using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Logics;
using AttendanceFetch.Helpers.MainServerFetchApi;
using AttendanceFetch.Helpers.Schedular;
using BiometricDataFetchAPI.Controllers;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BiometricDataFetchAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();// Specify the log file name
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddHangfire(x => x.UseMemoryStorage());

            ////services.AddHangfire(configuration => configuration
	           ////      .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
	           ////      .UseSimpleAssemblyNameTypeSerializer()
	           ////      .UseRecommendedSerializerSettings()
	           ////      .UseMemoryStorage()); // Use in-memory storage

			services.AddHangfireServer();

            var IsInMemoryRecurringJobRequired =  Configuration.GetSection("AppCustomSettings").GetSection("IsInMemoryRecurringJobRequired").Value;

            if (int.Parse(IsInMemoryRecurringJobRequired) == 1)
            {
                services.AddHostedService<RecurringJobService>();
                services.AddHostedService<RecurringJobFixTimeService>();
            }
            services.AddTransient<IZKTecoAttendance_DataFetchBL, ZKTecoAttendance_DataFetchBL>();
            services.AddTransient<IZKTecoAttendance_UserBL, ZKTecoAttendance_UserBL>();
            services.AddTransient<IJobSchedular , JobSchedular>();
            services.AddTransient<IAttendanceFetchBL, AttendanceFetchBL>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", null);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider service,
            IServiceScopeFactory serviceScopeFactory, ILogger<Startup> logger, ILoggerFactory loggerFactory )
        {

            //// Hangfire
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
            GlobalConfiguration.Configuration.UseMemoryStorage();
            var fetchIntervalTimeInMinutes = Configuration.GetSection("AppCustomSettings").GetSection("FetchTimeIntervalInMinutes").Value;



            var IsHangFireRequired = Configuration.GetSection("AppCustomSettings").GetSection("IsHangFireRequired").Value;

            if (int.Parse(IsHangFireRequired) == 1)
            {
                //RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoGetAttendance(), Cron.MinuteInterval(10), TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoPushDataToMainServer(), Cron.Hourly(Convert.ToInt32(fetchIntervalTimeInMinutes)), TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning(), Cron.Daily(8, 30), TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening(), Cron.Daily(19, 30), TimeZoneInfo.Local);

            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            loggerFactory.AddFile($@"{Directory.GetCurrentDirectory()}\logs\log.txt");
            logger.LogInformation($"-------------------------------------------------------------------------------------------------------------------------------------");

            logger.LogInformation($"----------Application started at {DateTime.Now}----------");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            //app.UseMiddleware<GlobalExceptionMiddleware>();
            //app.UseMvc();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "AreaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { area = "Core", controller = "Home", action = "Index" }
                );
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Biometric Device Attendance (V 1.0)");
            });

           
        }

	}
}
