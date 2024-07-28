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
    public class TestActivityScenario
    {
        private HttpClient _httpClient = new HttpClient();

        public ScenarioProps Create()
        {
            var user = new UserDto("LOGO", "LOGO");

            return Scenario
                .Create("test_activity_scenario", async context =>
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

                    var getAvailableActivity = await Step.Run<List<MTActivityResponse>>("get_available_activity", context, async () =>
                    {
                        var request = Http.CreateRequest("GET", $"http://localhost/LogoCRMRest/api/v1.0/activities?SessionId={sessionId}")
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
                            var updatedActivity = new MTActivityDto
                            {
                                Id = firstActivity.Id,
                                ActivitySubject = "UpdatedActivitySubject",
                                ActivityDate = DateTime.Now,
                                Priority = 1
                            };

                            var data = JsonConvert.SerializeObject(updatedActivity);
                            var request = Http.CreateRequest("PUT", $"http://localhost/LogoCRMRest/api/v1.0/activities/{firstActivity.Oid}?SessionId={sessionId}")
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
                            var request = Http.CreateRequest("GET", $"http://localhost/LogoCRMRest/api/v1.0/activities/{firstActivity.Oid}?SessionId={sessionId}")
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
                            var newActivity = new MTActivityDto
                            {
                                ActivitySubject = "NewActivitySubject",
                                ActivityDate = DateTime.Now,
                                Priority = 1
                            };
                            var data = JsonConvert.SerializeObject(newActivity);
                            var request = Http.CreateRequest("POST", $"http://localhost/LogoCRMRest/api/v1.0/activities?SessionId={sessionId}")
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
                            var request = Http.CreateRequest("GET", $"http://localhost/LogoCRMRest/api/v1.0/activities/{createdActivityPayload.Oid}?SessionId={sessionId}")
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
