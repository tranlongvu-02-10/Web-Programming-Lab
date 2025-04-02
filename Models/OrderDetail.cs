using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab04.WebsiteBanHang.Models
{
   public class OrderDetail 
{ 
    public int Id { get; set; } 
    public int OrderId { get; set; } 
    public int ProductId { get; set; } 
    public int Quantity { get; set; } 
    public decimal Price { get; set; } 
  
    public Order Order { get; set; } 
    public Product Product { get; set; } 
}
}