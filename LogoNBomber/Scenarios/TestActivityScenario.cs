﻿using LogoNBomber.Dtos;
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
    public class TestActivityScenario
    {
        private HttpClient _httpClient = new HttpClient();
        private const string endpoint = "activities";

        public ScenarioProps Create(UserDto user)
        {
            var faker = new MTActivityDtoFaker();

            return Scenario
                .Create($"{user.UserName}_test_activity_scenario", async context =>
                {
                var loginResponse = await ScenarioHelper.Login(user, context);

                    if (!loginResponse.IsError && loginResponse.Payload.IsSome())
                    {

                        string sessionId = loginResponse.Payload.Value;

                        var getAvailableActivity = await Step.Run<List<MTActivityResponse>>("get_available_activity", context, async () =>
                        {
                            var request = Http.CreateRequest(Constants.GET, $"{Constants.BaseUrl}{endpoint}?SessionId={sessionId}")
                                .WithHeader("Content-Type", "application/json");

                            var response = await Http.Send(_httpClient, request);

                            if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Payload.Value.Content.ReadAsStringAsync();
                                var json = JsonConvert.DeserializeObject<dynamic>(jsonString);
                                var activitiesList = JsonConvert.DeserializeObject<List<MTActivityResponse>>(json.Items.ToString());

                                return Response.Ok(payload: activitiesList, sizeBytes: response.SizeBytes);
                            }
                            else
                            {
                                return Response.Fail<List<MTActivityResponse>>();
                            }
                        });

                        var activities = getAvailableActivity.Payload.Value;

                        if (activities.Any())
                        {
                            var firstActivity = activities.First();

                            var updateActivity = await Step.Run("update_activity", context, async () =>
                            {
                                var updatedActivity = faker.Generate();

                                var data = JsonConvert.SerializeObject(updatedActivity);
                                var request = Http.CreateRequest(Constants.PUT, $"{Constants.BaseUrl}{endpoint}/{firstActivity.Oid}?SessionId={sessionId}")
                                    .WithHeader("Content-Type", "application/json")
                                    .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                                var response = await Http.Send(_httpClient, request);

                                if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                                {
                                    return Response.Ok(payload: updatedActivity);
                                }
                                else
                                {
                                    return Response.Fail<MTActivityDto>();
                                }
                            });

                            var updatedActivityPayload = updateActivity.Payload.Value;

                            var getUpdatedActivity = await Step.Run("get_updated_activity", context, async () =>
                            {
                                var request = Http.CreateRequest(Constants.GET, $"{Constants.BaseUrl}{endpoint}/{firstActivity.Oid}?SessionId={sessionId}")
                                    .WithHeader("Content-Type", "application/json");

                                var response = await Http.Send(_httpClient, request);

                                if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                                {
                                    var activity = await response.Payload.Value.Content.ReadFromJsonAsync<MTActivityDto>();

                                    return Response.Ok(payload: activity);
                                }
                                else
                                {
                                    return Response.Fail<MTActivityDto>();
                                }
                            });
                        }
                        else
                        {
                            var createActivity = await Step.Run("create_activity", context, async () =>
                            {
                                var newActivity = faker.Generate();
                                var data = JsonConvert.SerializeObject(newActivity);
                                var request = Http.CreateRequest(Constants.POST, $"{Constants.BaseUrl}{endpoint}?SessionId={sessionId}")
                                    .WithHeader("Content-Type", "application/json")
                                    .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                                var response = await Http.Send(_httpClient, request);

                                if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                                {
                                    var createdActivity = await response.Payload.Value.Content.ReadFromJsonAsync<MTActivityResponse>();

                                    return Response.Ok(payload: createdActivity);
                                }
                                else
                                {
                                    return Response.Fail<MTActivityResponse>();
                                }
                            });

                            var createdActivityPayload = createActivity.Payload.Value;

                            var getCreatedActivity = await Step.Run("get_created_activity", context, async () =>
                            {
                                var request = Http.CreateRequest(Constants.GET, $"{Constants.BaseUrl}{endpoint}/{createdActivityPayload.Oid}?SessionId={sessionId}")
                                    .WithHeader("Content-Type", "application/json");

                                var response = await Http.Send(_httpClient, request);

                                if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                                {
                                    var activity = await response.Payload.Value.Content.ReadFromJsonAsync<MTActivityResponse>();

                                    return Response.Ok(payload: activity);
                                }
                                else
                                {
                                    return Response.Fail<MTActivityResponse>();
                                }
                            });
                        }

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
