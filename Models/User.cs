using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_api.Models
{
    public class User
    {
        public int Id { get; set; }   
        public string UserName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new Byte[64];
        public byte[] PasswordSalt { get; set; } = new Byte [64];
    }
}