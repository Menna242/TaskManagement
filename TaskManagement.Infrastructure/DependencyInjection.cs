using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Infrastructure.Authentication;
using TaskManagement.Infrastructure.Identity;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Infrastructure.Authentication;
using Hangfire;
using TaskManagement.Application.Abstractions.BackgroundJobs;
using TaskManagement.Infrastructure.BackgroundJobs;
using Quartz;

namespace TaskManagement.Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            {
                Attempts = 3,
                DelaysInSeconds = new[] { 10, 30, 60 },
                OnAttemptsExceeded = AttemptsExceededAction.Fail
            });

            services.AddHangfireServer();

            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("OverdueTasksJob");

                q.AddJob<OverdueTasksJob>(options => options.WithIdentity(jobKey));

                q.AddTrigger(trigger => trigger
                    .ForJob(jobKey)
                    .WithIdentity("OverdueTasksTrigger")
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInMinutes(1)
                        .RepeatForever()));
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            return services;
        }
    }

}
