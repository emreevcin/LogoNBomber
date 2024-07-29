using LogoNBomber.Dtos;
using LogoNBomber.Fakers;
using LogoNBomber.Generics;
using LogoNBomber.Helpers;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Scenarios
{
    public class TestProposalScenario
    {
        private HttpClient _httpClient = new HttpClient();
        private const string endpoint = "proposals";
        public ScenarioProps Create(UserDto user)
        {
            var faker = new MTProposalDtoFaker();

            return Scenario
                .Create($"{user.UserName}_test_proposal_scenario", async context =>
                {
                var loginResponse = await ScenarioHelper.Login(user, context);
                if (!loginResponse.IsError && loginResponse.Payload.IsSome())
                {
                        string sessionId = loginResponse.Payload.Value;

                        var createProposal = await Step.Run($"{user.UserName}_create_proposal", context, async () =>
                        {
                            var data = JsonConvert.SerializeObject(faker.Generate());
                            var request = Http.CreateRequest(Constants.POST, $"{Constants.BaseUrl}{endpoint}?SessionId={sessionId}")
                                .WithHeader("Content-Type", "application/json")
                                .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                            var response = await Http.Send(_httpClient, request);

                            if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                            {
                                var responseBody = await response.Payload.Value.Content.ReadAsStringAsync();
                                var responseDto = JsonConvert.DeserializeObject<MTProposalResponse>(responseBody);
                                return Response.Ok(payload: responseDto);
                            }
                            else
                            {
                                return Response.Fail<MTProposalResponse>();
                            }
                        });


                        var createdProposalPayload = createProposal.Payload.Value;

                        var deleteProposal = await Step.Run($"{user.UserName}_delete_proposal", context, async () =>
                        {
                            var request = Http.CreateRequest(Constants.DELETE, $"{Constants.BaseUrl}{endpoint}/{createdProposalPayload.Oid}?SessionId={sessionId}")
                                .WithHeader("Content-Type", "application/json");

                            var response = await Http.Send(_httpClient, request);

                            return response;
                        });

                        var logout = await ScenarioHelper.Logout(user, context);

                        return logout.IsError ? Response.Fail() : Response.Ok();
                    }
                    else
                    {
                        context.Logger.Error("Login failed or payload is empty.");
                        return Response.Fail();
                    };
                });
        }
    }
}
