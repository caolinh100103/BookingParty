using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }

        public virtual ICollection<ServiceItem> ServiceItems { get; set; }
    }
}
