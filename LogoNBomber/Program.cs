using LogoNBomber.Dtos;
using LogoNBomber.Helpers;
using LogoNBomber.Scenarios;
using Microsoft.Extensions.DependencyInjection;
using NBomber.Contracts;
using NBomber.CSharp;
using System;
using System.Collections.Generic;
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

            //var testLoginLogoutScenario = new TestLoginLogoutScenario();
            //var testActivityScenario = new TestActivityScenario();
            var testFirmScenario = new TestFirmScenario();
            //var testTicketScenario = new TestTicketScenario();
            //var testProposalScenario = new TestProposalScenario();

            var scenarios = new List<ScenarioProps>();
            var users = await FetchActiveUsers();
            foreach (var user in users)
            {
                scenarios.Add( testFirmScenario.Create(user)
                    .WithLoadSimulations(Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)))
                    .WithoutWarmUp());

            }
           
            NBomberRunner.RegisterScenarios(scenarios.ToArray())
                .Run();
            //var tasks = new List<Task>();

            //foreach (var user in users)
            //{
            //    var task = Task.Run(async () =>
            //    {
            //        var loginLogoutScenario = testLoginLogoutScenario.Create(user)
            //            .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //            .WithoutWarmUp();

            //        var activityScenario = testActivityScenario.Create(user)
            //            .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //            .WithoutWarmUp();

            //        var firmScenario = testFirmScenario.Create(user)
            //            .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //            .WithoutWarmUp();

            //        var ticketScenario = testTicketScenario.Create(user)
            //            .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //            .WithoutWarmUp();

            //        var proposalScenario = testProposalScenario.Create(user)
            //            .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //            .WithoutWarmUp();

            //        await RunScenarioWithDelay(loginLogoutScenario, TimeSpan.FromSeconds(3));
            //        await RunScenarioWithDelay(activityScenario, TimeSpan.FromSeconds(3));
            //        await RunScenarioWithDelay(firmScenario, TimeSpan.FromSeconds(3));
            //        await RunScenarioWithDelay(ticketScenario, TimeSpan.FromSeconds(3));
            //        await RunScenarioWithDelay(proposalScenario, TimeSpan.FromSeconds(3));
            //    });

            //    tasks.Add(task);
            //}

            //await Task.WhenAll(tasks);



            //foreach (var user in users)
            //{
            //    var loginLogoutScenario = testLoginLogoutScenario.Create(user)
            //             .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //             .WithoutWarmUp();

            //    await Task.Delay(TimeSpan.FromSeconds(3));

            //    var activityScenario = testActivityScenario.Create(user)
            //        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //        .WithoutWarmUp();

            //    await Task.Delay(TimeSpan.FromSeconds(3));

            //    var firmScenario = testFirmScenario.Create(user)
            //        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //        .WithoutWarmUp();

            //    await Task.Delay(TimeSpan.FromSeconds(3));

            //    var ticketScenario = testTicketScenario.Create(user)
            //        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //        .WithoutWarmUp();

            //    await Task.Delay(TimeSpan.FromSeconds(3));

            //    var proposalScenario = testProposalScenario.Create(user)
            //        .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)))
            //        .WithoutWarmUp();

            //    await Task.Delay(TimeSpan.FromSeconds(3));

            //    NBomberRunner.RegisterScenarios(loginLogoutScenario, activityScenario, firmScenario, ticketScenario, proposalScenario)
            //        .Run();


            //}
            
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        //private static async Task RunScenarioWithDelay(ScenarioProps scenario, TimeSpan delay)
        //{
        //    var result = NBomberRunner
        //        .RegisterScenarios(scenario)
        //        .Run();

        //    await Task.Delay(delay);
        //}

        private static async Task<List<UserDto>> FetchActiveUsers()
        {
            var user = new UserDto("LOGO", "LOGO");
            var httpClient = new HttpClient();

            try
            {
                List<UserDto> users = await AuthenticationHelper.GetActiveUsers(user, httpClient);
                return users;
            }
            catch
            {
                return new List<UserDto>();
            }
        }
    }
}
