using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Dtos.User;
using rpg_api.Services.AuthService;

namespace rpg_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authRepo;
        public AuthController(IAuthService authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {
            var response = await _authRepo.Register(new User{ UserName = request.Username }, request.Password);
            if(!response.Success){
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);
            if(!response.Success){
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}