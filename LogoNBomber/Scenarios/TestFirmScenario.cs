﻿using LogoNBomber.Dtos;
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
    public class TestFirmScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create(UserDto user)
        {
            return Scenario
                .Create($"{user.UserName}_test_firm_scenario", async context =>
                {
                    var loginResponse = await ScenarioHelper.Login(user, context);

                    if (!loginResponse.IsError && loginResponse.Payload.IsSome())
                    {

                        string sessionId = loginResponse.Payload.Value;

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

                        var logout = await ScenarioHelper.Logout(user, context);

                        return logout.IsError ? Response.Fail() : Response.Ok();
                    }
                    else
                    {
                        context.Logger.Error("Login failed or payload is empty.");
                        return Response.Fail();
                    }
                });
        }
    }
}
