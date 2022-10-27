using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rpg_api.Services.GlobalService
{
    public class GlobalService: IGlobalService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        public GlobalService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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