using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ServiceResponse<FightResultDto>> AutoFight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>{
                Data = new FightResultDto()
            };

            try {
                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();
                
                foreach(Character character in characters)
                {
                    //TODO: allow only characters owned by user
                    if(character.Skills == null || character.Weapon == null)
                    {
                        response.Success = false;
                        response.Message = "You should only enter in a auto fight with characters that have a weapon and at least one skill!";
                        return response;
                    }
                }

                bool onlyOneLeft = false;
                while(!onlyOneLeft) {
                    foreach(Character attacker in characters.ToList())
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if(useWeapon && attacker.Weapon != null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else if(attacker.Skills != null)
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage > 0 ? damage : 0)} damage.");
                        if(opponent.HitPoints <= 0)
                        {   
                            characters.Remove(opponent);
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");

                            if(characters.Count == 1){
                                onlyOneLeft = true;
                                attacker.Victories++;
                                response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                                break;
                            }
                        }
                    }
                }

                //TODO: not saving
                characters.ToList().ForEach(c => {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                await _context.SaveChangesAsync();

            } catch (Exception ex) {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId && c.User.Id == _globalService.GetUserId());

                var opponent = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if(attacker == null || opponent == null){
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                var skill = attacker.Skills?.FirstOrDefault(s => s.Id == request.SkillId);
                if(skill == null){
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know this skill";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);
                
                if(opponent.HitPoints <= 0){
                    response.Message = $"{opponent.Name} has been defeated!";
            
                    opponent.Fights++;
                    attacker.Fights++;
                    opponent.Defeats++;
                    attacker.Victories++;
                }
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
                return response;
            } catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }        
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId && c.User.Id == _globalService.GetUserId());

                var opponent = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if(attacker == null || opponent == null){
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }
                if(attacker.Weapon == null){
                    response.Success = false;
                    response.Message = "The attacker doesn't have a weapon";
                    return response;
                }

                int damage = DoWeaponAttack(attacker, opponent);

                if(opponent.HitPoints <= 0){
                    response.Message = $"{opponent.Name} has been defeated!";

                    opponent.Fights++;
                    attacker.Fights++;
                    opponent.Defeats++;
                    attacker.Victories++;
                }
                
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
                return response;
            } catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public int DoWeaponAttack(Character attacker, Character opponent){
            if(attacker.Weapon == null) {
                return 0;
            }

            int damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength) + new Random().Next(attacker.Intelligence);
            damage -= new Random().Next(opponent.Defense);
            if(damage > 0){
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public int DoSkillAttack(Character attacker, Character opponent, Skill skill){
            int damage = skill.Damage + new Random().Next(attacker.Strength) + new Random().Next(attacker.Intelligence);
            damage -= new Random().Next(opponent.Defense);
            if(damage > 0){
                opponent.HitPoints -= damage;
            }
            return damage;
        }
    }
}