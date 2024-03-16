using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public int? ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public int? RoomId { get; set; } 
        public virtual Room Room{ get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
