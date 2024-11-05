using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Coffee_PryStore.Models
{
    public class OrderItemViewModel
    {
        public int CofId { get; set; }  // Ідентифікатор товару
        public int Quantity { get; set; }  // Кількість товару
        public decimal UnitPrice { get; set; }  // Ціна за одиницю
    }
}