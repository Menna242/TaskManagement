using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using TaskManagement.Application.Abstractions.BackgroundJobs;
using TaskManagement.Infrastructure;

namespace TaskManagement.Tests.Integration;

public class ApiFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
       "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDb_Tests;Trusted_Connection=True;MultipleActiveResultSets=true";


    public ApiFactory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(TestConnectionString)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", TestConnectionString);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IBackgroundJobService>();
            services.AddScoped(_ => Substitute.For<IBackgroundJobService>());
        });
    }
}