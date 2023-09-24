using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Logics;
using AttendanceFetch.Helpers.MainServerFetchApi;
using AttendanceFetch.Helpers.Schedular;
using BiometricDataFetchAPI.Controllers;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

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
            IServiceScopeFactory serviceScopeFactory)
        {

            // Hangfire
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
            var fetchIntervalTimeInMinutes = Configuration.GetSection("AppCustomSettings").GetSection("FetchTimeIntervalInMinutes").Value;

            //RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoGetAttendance(), Cron.MinuteInterval(10), TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoPushDataToMainServer(), Cron.Hourly(Convert.ToInt32(fetchIntervalTimeInMinutes)), TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning(), Cron.Daily(8,30), TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => service.GetRequiredService<IJobSchedular>().ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening(), Cron.Daily(19,00), TimeZoneInfo.Local);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Biometric Device Attendance (V 1.0)");
            });
        }
    }
}
