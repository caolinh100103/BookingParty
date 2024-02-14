using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }
        public float MaxReduction { get; set; }
        public int ReductionPercent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Sattus { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service{ get; set; }
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}
