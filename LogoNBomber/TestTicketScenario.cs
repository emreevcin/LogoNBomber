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
    public class TestTicketScenario
    {
        private HttpClient _httpClient = new HttpClient();

        public ScenarioProps Create()
        {
            var user = new UserDto("LOGO", "LOGO");

            return Scenario
                .Create("test_ticket_scenario", async context =>
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

                    // Şikayet olan ticketları çek
                    var getTickets = await Step.Run("get_tickets", context, async () =>
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

                    // Gelen listeden 3 tane ticket seç ve bunları güncelle
                    var ticketsToUpdate = ticketsPayload.Take(3).ToList();
                    foreach (var ticket in ticketsToUpdate)
                    {
                        var updateTicket = await Step.Run("update_ticket", context, async () =>
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