using LogoNBomber.Dtos;
using NBomber.Contracts;
using NBomber.CSharp;
using System;

namespace LogoNBomber.Helpers
{
    public class ScenarioHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<Response<string>> Login(UserDto user, IScenarioContext context)
        {
            return await Step.Run("login", context, async () =>
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
        }

        public static async Task<Response<string>> Logout(UserDto user, IScenarioContext context)
        {
            return await Step.Run("logout", context, async () =>
            {
                try
                {
                    await AuthenticationHelper.Logout(user, _httpClient);
                    return Response.Ok(payload: string.Empty);
                }
                catch (Exception ex)
                {
                    context.Logger.Error(ex, "Logout failed");
                    return Response.Fail<string>();
                }
            });
        }
    }
}
