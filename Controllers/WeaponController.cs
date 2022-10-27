using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.Weapon;
using rpg_api.Services.WeaponService;

namespace rpg_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeaponController: ControllerBase
    {   
        private readonly IWeaponService _weaponService;
        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddWeapon(AddWeaponDto newWeapon)
        {
            var response = await _weaponService.AddWeapon(newWeapon);
            if(response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}