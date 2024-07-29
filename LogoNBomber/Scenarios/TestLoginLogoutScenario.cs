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
    public class TestLoginLogoutScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            var user = new UserDto("LOGO", "LOGO");

            return Scenario
                .Create("login_logout_scenario", async context =>
                {
                    var loginResponse = await ScenarioHelper.Login(user, context);

                    if (!loginResponse.IsError && loginResponse.Payload.IsSome())
                    {
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
