using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Domain.Entities
{

    public class AthleteAchievement : BaseEntity
    {
        public long AthleteId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }

        public AthleteProfile Athlete { get; set; } = null!;
    }
}
