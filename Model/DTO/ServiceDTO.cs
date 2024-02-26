using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO;
public class ServiceDTO
{
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
}
