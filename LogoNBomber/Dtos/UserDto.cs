using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Dtos
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
}
