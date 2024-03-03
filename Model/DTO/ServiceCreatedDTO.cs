namespace Model.DTO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
public class ServiceCreatedDTO
 {
     public string ServiceTitle { get; set; }
     public string ServiceName { get; set; }
     public decimal Price { get; set; }
     public string Description { get; set; }
     public int UserId { get; set; }
     public int CategoryId { get; set; }
     public List<IFormFile> Images { get; set; }
 }