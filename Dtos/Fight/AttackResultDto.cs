using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_api.Dtos.Fight
{
    public class AttackResultDto
    {
        public string AttackerName { get; set; } = string.Empty;
        public string OpponentName { get; set; } = string.Empty;
        public int AttackerHP { get; set; }
        public int OpponentHP { get; set; }
        public int Damage { get; set; }
    }
}