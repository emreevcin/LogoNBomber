using Microsoft.Extensions.DependencyInjection;
using NBomber.CSharp;
using System.Net.Http;

namespace LogoNBomber
{
    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();

            var testActivityScenario = new TestActivityScenario();

            var scenario = testActivityScenario.Create()
                .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(1)))
                .WithoutWarmUp();
            var result = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }
    }
}