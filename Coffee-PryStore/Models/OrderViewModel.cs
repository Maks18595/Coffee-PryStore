using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Coffee_PryStore.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть ПІБ")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть місто доставки")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть адресу доставки")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]
        [Phone(ErrorMessage = "Некоректний номер телефону")]
        public string PhoneNumber { get; set; } = string.Empty;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
