using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Coffee_PryStore.Models
{
    public class OrderItemViewModel
    {
        public int CofId { get; set; }  
        public int Quantity { get; set; }  
        public decimal UnitPrice { get; set; }  
    }
}