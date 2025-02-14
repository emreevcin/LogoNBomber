﻿namespace LogoNBomber.Dtos
{
    public class UserDto
    {
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public UserDto(string userName, string password)
        {
            SessionId = null;
            UserName = userName;
            Password = password;
        }
    }

    public class UserResponseDto
    {
        public List<UserDto> Items { get; set; }
    }
}
