using LogoNBomber.Dtos;
using LogoNBomber.Fakers;
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

namespace LogoNBomber
{
    public class TestProposalScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            var user = new UserDto("LOGO", "LOGO");
            var faker = new MTProposalDtoFaker();

            return Scenario
                .Create("test_proposal_scenario", async context =>
                {
                    var login = await Step.Run("login", context, async () =>
                    {
                        try
                        {
                            var sessionId = await AuthenticationHelper.Login(user, _httpClient);

                            return Response.Ok(payload: sessionId);
                        }
                        catch (Exception ex)
                        {
                            context.Logger.Error(ex, "Login failed");
                            return Response.Fail<string>();
                        }
                    });

                    string sessionId = login.Payload.Value;

                    var createProposal = await Step.Run("create_proposal", context, async () =>
                    {
                        var data = JsonConvert.SerializeObject(faker.Generate());
                        var request = Http.CreateRequest("POST", $"http://localhost/LogoCRMRest/api/v1.0/proposals?SessionId={sessionId}")
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

                    var deleteProposal = await Step.Run("delete_proposal", context, async () =>
                    {
                        var request = Http.CreateRequest("DELETE", $"http://localhost/LogoCRMRest/api/v1.0/proposals/{createdProposalPayload.Oid}?SessionId={sessionId}")
                            .WithHeader("Content-Type", "application/json");

                        var response = await Http.Send(_httpClient, request);

                        return response;
                    });

                    var logout = await Step.Run("logout", context, async () =>
                    {
                        await AuthenticationHelper.Logout(user, _httpClient);
                        return Response.Ok();
                    });

                    return Response.Ok();
                });
        }
    }
}
