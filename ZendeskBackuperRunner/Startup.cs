using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.LiteDB;
using Hangfire.Dashboard;
using System;

namespace ZendeskBackuper.Runner
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                .UseLiteDbStorage()
            );

            services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs)
        {
            app.UseHangfireServer();
            // цепляем дашборд на корень сайта
            app.UseHangfireDashboard("", new DashboardOptions
            {
                //IsReadOnlyFunc = (DashboardContext context) => true
            });
            // бекапим
            RecurringJob.AddOrUpdate(() => Backuper.Program.Run(), "0 0 * * *", TimeZoneInfo.Local);
        }
    }
}
