using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg_api.Data;
using rpg_api.Dtos.Character;

namespace rpg_api.Services.GlobalService
{
    public class GlobalService: IGlobalService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public GlobalService
        (
            IHttpContextAccessor httpContextAccessor,
            DataContext context,
            IMapper mapper
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public int? GetUserId() {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(value == null){
                return null;
            }
            return int.Parse(value);
        }
    }
}