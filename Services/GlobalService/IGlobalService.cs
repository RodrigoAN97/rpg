using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rpg_api.Dtos.Character;

namespace rpg_api.Services.GlobalService
{
    public interface IGlobalService
    {
        int? GetUserId();
    }
}