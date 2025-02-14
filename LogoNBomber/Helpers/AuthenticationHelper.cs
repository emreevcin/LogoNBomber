﻿using NBomber.Http.CSharp;
using LogoNBomber.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LogoNBomber.Generics;

namespace LogoNBomber.Helpers
{
    public static class AuthenticationHelper
    {
        public static async Task<string> Login(UserDto user, HttpClient _httpClient)
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.UserName}:{user.Password}"));
            var url = $"{Constants.BaseUrl}login?authorization={auth}";

            var request = Http.CreateRequest(Constants.POST, url);
            var response = await Http.Send(_httpClient, request);

            if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
            {
                var responseContent = await response.Payload.Value.Content.ReadAsStringAsync();
                var userDto = JsonConvert.DeserializeObject<UserDto>(responseContent);

                return userDto.SessionId;
            }
            else
            {
                throw new HttpRequestException("Login failed");
            }
        }

        public static async Task Logout(UserDto user, HttpClient _httpClient)
        {
            var url = $"{Constants.BaseUrl}logout?sessionId={user.SessionId}";
            var request = Http.CreateRequest(Constants.POST, url);

            await Http.Send(_httpClient, request);
        }

        public static async Task<List<UserDto>> GetActiveUsers(UserDto user, HttpClient _httpClient)
        {
            var sessionId = await Login(user, _httpClient);
            var requestUrl = $"{Constants.BaseUrl}users?SessionId={sessionId}&query=isActive=true";

            var request = Http.CreateRequest(Constants.GET, requestUrl);
            var response = await Http.Send(_httpClient, request);

            if (!response.IsError && response.Payload.Value.IsSuccessStatusCode)
            {
                var responseContent = await response.Payload.Value.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<UserResponseDto>(responseContent);

                var userDtos = userResponse.Items.Select(u => new UserDto(u.UserName, u.UserName)).ToList();
                return userDtos;
            }
            else
            {
                throw new HttpRequestException("GetActiveUsers failed");
            }
        }
    }
}
