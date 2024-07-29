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

namespace LogoNBomber.Scenarios
{
    public class TestTicketScenario
    {
        private HttpClient _httpClient = new HttpClient();

        public ScenarioProps Create(UserDto user)
        {
            return Scenario
                .Create($"{user.UserName}_test_ticket_scenario", async context =>
                {
                    var loginResponse = await ScenarioHelper.Login(user, context);

                    if (!loginResponse.IsError && loginResponse.Payload.IsSome())
                    {
                        string sessionId = loginResponse.Payload.Value;

                        var getTickets = await Step.Run($"{user.UserName}_get_tickets", context, async () =>
                        {
                            var request = Http.CreateRequest("GET", $"http://localhost/LogoCRMRest/api/v1.0/tickets?SessionId={sessionId}&query=TicketDescription='Şikayet'")
                                .WithHeader("Content-Type", "application/json");

                            var response = await Http.Send(_httpClient, request);

                            if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                            {
                                var responseBody = await response.Payload.Value.Content.ReadAsStringAsync();
                                var ticketResponse = JsonConvert.DeserializeObject<TicketItemsResponse>(responseBody);
                                return Response.Ok(payload: ticketResponse.Items);
                            }
                            else
                            {
                                return Response.Fail<List<MTTicketDto>>();
                            }
                        });

                        var ticketsPayload = getTickets.Payload.Value;

                        var ticketsToUpdate = ticketsPayload.ToList();
                        foreach (var ticket in ticketsToUpdate)
                        {
                            var updateTicket = await Step.Run($"{user.UserName}_update_ticket", context, async () =>
                            {
                                ticket.TicketDescription = "Müşteri Şikayeti";
                                var data = JsonConvert.SerializeObject(ticket);
                                var request = Http.CreateRequest("PATCH", $"http://localhost/LogoCRMRest/api/v1.0/tickets/{ticket.Oid}?SessionId={sessionId}")
                                    .WithHeader("Content-Type", "application/json")
                                    .WithBody(new StringContent(data, Encoding.UTF8, "application/json"));

                                var response = await Http.Send(_httpClient, request);

                                if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
                                {
                                    return Response.Ok();
                                }
                                else
                                {
                                    return Response.Fail();
                                }
                            });

                            if (updateTicket.IsError)
                            {
                                return Response.Fail();
                            }
                        }

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