using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_api.Dtos.Character.User
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}