using LogoNBomber.Dtos;
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
    public class TestFirmScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            var user = new UserDto("LOGO", "LOGO");

            return Scenario
                .Create("test_firm_scenario", async context =>
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

                    var getAvailableFirm = await Step.Run("get_available_firm", context, async () =>
                    { 
                        var request = Http.CreateRequest("GET", $"http://localhost/LogoCRMRest/api/v1.0/firms?SessionId={sessionId}")
                            .WithHeader("Content-Type", "application/json");

                        var response = await Http.Send(_httpClient, request);

                        if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                        {
                            var firms = response.Payload.Value.Content;
                            var firmsList = firms.ReadFromJsonAsync<HttpResponse<List<MTFirmResponse>>>().Result.Data;

                            return Response.Ok(payload: firmsList, sizeBytes: response.SizeBytes);
                        }
                        else
                        {
                            return Response.Fail<List<MTFirmResponse>>();
                        }
                    });

                    var firms = getAvailableFirm.Payload.Value;

                    var createFirm = await Step.Run("create_firm", context, async () =>
                    {

                        var data = JsonConvert.SerializeObject(new MTFirmDto
                        {
                            FirmCode = "FirmCode",
                            FirmTitle = "FirmTitle",
                            InUse = true
                        });
                        var request = Http.CreateRequest("POST", $"http://localhost/LogoCRMRest/api/v1.0/firms?SessionId={sessionId}")
                            .WithHeader("Content-Type", "application/json")
                            .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

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
