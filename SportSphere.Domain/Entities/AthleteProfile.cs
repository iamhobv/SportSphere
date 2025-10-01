using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Domain.Entities
{
    [Index(nameof(Sport), IsUnique = false)]
    public class AthleteProfile : SocialProfile
    {

        public string Sport { get; set; } = null!;
        public string? Position { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }
        public bool IsVerified { get; set; } = false;
        public ICollection<AthleteAchievement>? AthleteAchievements { get; set; } = new List<AthleteAchievement>();


    }
}
