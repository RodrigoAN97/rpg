using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.Weapon;

namespace rpg_api.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}