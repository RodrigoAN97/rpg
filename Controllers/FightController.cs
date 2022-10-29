using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Services.FightService;

namespace rpg_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("controller")]
    public class FightController: ControllerBase
    {
        private readonly IFightService _fightService;
        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }               
    }
}