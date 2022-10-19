using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using rpg_api.Dtos.Character;

namespace rpg_api
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<Character, AddCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
        }
    }
}