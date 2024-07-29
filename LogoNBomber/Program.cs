using LogoNBomber.Scenarios;
using Microsoft.Extensions.DependencyInjection;
using NBomber.Contracts;
using NBomber.CSharp;
using System.Net.Http;
using System.Threading.Tasks;

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

            var testLoginLogoutScenario = new TestLoginLogoutScenario();
            var testActivityScenario = new TestActivityScenario();
            var testFirmScenario = new TestFirmScenario();
            var testTicketScenario = new TestTicketScenario();
            var testProposalScenario = new TestProposalScenario();

            var loginLogoutScenario = testLoginLogoutScenario.Create()
                .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)))
                .WithoutWarmUp();

            var activityScenario = testActivityScenario.Create()
                .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)))
                .WithoutWarmUp();

            var firmScenario = testFirmScenario.Create()
                .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)))
                .WithoutWarmUp();

            var ticketScenario = testTicketScenario.Create()
                .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)))
                .WithoutWarmUp();

            var proposalScenario = testProposalScenario.Create()
                .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)))
                .WithoutWarmUp();

            // Run each scenario sequentially with delay between them
            await RunScenarioWithDelay(loginLogoutScenario, TimeSpan.FromSeconds(3));
            await RunScenarioWithDelay(activityScenario, TimeSpan.FromSeconds(3));
            await RunScenarioWithDelay(firmScenario, TimeSpan.FromSeconds(3));
            await RunScenarioWithDelay(ticketScenario, TimeSpan.FromSeconds(3));
            await RunScenarioWithDelay(proposalScenario, TimeSpan.FromSeconds(3));
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        private static async Task RunScenarioWithDelay(ScenarioProps scenario, TimeSpan delay)
        {
            var result = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            await Task.Delay(delay);
        }
    }
}
