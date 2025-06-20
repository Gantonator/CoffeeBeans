using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoffeeBeanApi.Data;

namespace CoffeeBeanApi.IntegrationTests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d => d.ServiceType?.FullName?.Contains("EntityFramework") == true).ToArray())
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<CoffeeBeanContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDatabase");
            });

            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        });

        builder.UseEnvironment("Testing");
    }
}