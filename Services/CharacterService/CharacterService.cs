using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using rpg_api.Data;
using rpg_api.Dtos.Character;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using rpg_api.Services.GlobalService;

namespace rpg_api.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGlobalService _globalService;
        public CharacterService
        (
            IMapper mapper, 
            DataContext context, 
            IHttpContextAccessor httpContextAccessor, 
            IGlobalService globalService
        )
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _globalService = globalService;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == _globalService.GetUserId());
            if(dbUser == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User not found";
            }
            else
            {
                character.User = dbUser;
                _context.Characters.Add(character);
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters
                    .Where(c => c.User.Id == _globalService.GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try {
                Character? character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == _globalService.GetUserId());

                if(character == null){
                    response.Success = false;
                    response.Message = "Character not found";
                } else {
                    var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);

                    if(skill == null){
                        response.Success = false;
                        response.Message = "Skill not found";
                    } else {
                        character.Skills?.Add(skill);
                        await _context.SaveChangesAsync();
                        response.Data = _mapper.Map<GetCharacterDto>(character);
                    }
                }

            } catch(Exception ex) {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try {
                var dbCharacter = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == _globalService.GetUserId());
                if(dbCharacter != null)
                {
                    _context.Characters.Remove(dbCharacter);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = _context.Characters
                        .Where(c => c.User.Id == _globalService.GetUserId())
                        .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }

            } catch(Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;        
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.User.Id == _globalService.GetUserId())
                .ToListAsync();
            response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == _globalService.GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

            try {
                var dbCharacter = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id && c.User.Id == _globalService.GetUserId());
                if(dbCharacter == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }
                else
                {
                    dbCharacter.Name = updatedCharacter.Name;
                    dbCharacter.HitPoints = updatedCharacter.HitPoints;
                    dbCharacter.Strength = updatedCharacter.Strength;
                    dbCharacter.Defense = updatedCharacter.Defense;
                    dbCharacter.Intelligence = updatedCharacter.Intelligence;
                    dbCharacter.Class = updatedCharacter.Class;

                    await _context.SaveChangesAsync();

                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
                }
            } catch(Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;
        }
    }
}