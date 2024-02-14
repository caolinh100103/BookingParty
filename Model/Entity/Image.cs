using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
        public int Status { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}
