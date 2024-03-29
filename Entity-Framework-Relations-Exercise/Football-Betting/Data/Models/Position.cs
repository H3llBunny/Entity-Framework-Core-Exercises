﻿using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Data.Models
{
    public class Position
    {
        public Position()
        {
            this.Players = new HashSet<Player>();
        }

        [Key]
        public int PositionId { get; set; }

        public string Name { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
