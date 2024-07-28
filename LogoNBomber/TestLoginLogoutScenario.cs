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
    public class TestLoginLogoutScenario
    {
        private HttpClient _httpClient = new HttpClient();
        public ScenarioProps Create()
        {
            var user = new UserDto("LOGO", "LOGO");

            return Scenario
                .Create("login_logout_scenario", async context =>
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
