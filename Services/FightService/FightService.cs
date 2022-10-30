using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rpg_api.Data;
using rpg_api.Dtos.Fight;
using rpg_api.Services.CharacterService;
using rpg_api.Services.GlobalService;

namespace rpg_api.Services.FightService
{
    public class FightService: IFightService
    {
        private readonly DataContext _context;
        private readonly ICharacterService _characterService;
        private readonly IGlobalService _globalService;
        public FightService
        (
            DataContext context,
            ICharacterService characterService,
            IGlobalService globalService
        )
        {
            _context = context;
            _characterService = characterService;
            _globalService = globalService;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try {
                var attacker = await _characterService.GetCharacterById(request.AttackerId);

                var opponent = await _globalService.GetOpponentCharacterById(request.OpponentId);

                if(attacker.Data == null || opponent.Data == null){
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }
                if(attacker.Data.Weapon == null){
                    response.Success = false;
                    response.Message = "The attacker doesn't have a weapon";
                    return response;
                }

                // TODO: write another method to calculate the damage, use intelligence or delete this prop
                //TODO: save fights, defeats and victories to the characters context
                int damage = attacker.Data.Weapon.Damage + (new Random().Next(attacker.Data.Strength));
                damage -= new Random().Next(opponent.Data.Defense);
                if(damage > 0){
                    opponent.Data.HitPoints -= damage;
                }
                if(opponent.Data.HitPoints < 0){
                    response.Message = $"{opponent.Data.Name} has been defeated!";
                }
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Data.Name,
                    OpponentName = opponent.Data.Name,
                    AttackerHP = attacker.Data.HitPoints,
                    OpponentHP = opponent.Data.HitPoints,
                    Damage = damage
                };
                return response;
            } catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}