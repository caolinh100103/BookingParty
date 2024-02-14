using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class ServiceItem
    {
        [ForeignKey("Item")]
        public int ItemId { get;set; }
        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        public Item Item { get; set; }
        public Service Service { get; set; }
    }
}
