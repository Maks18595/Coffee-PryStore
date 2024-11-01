using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffee_PryStore.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        // Navigation property
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public User User { get; set; } = null!;  // Reference to the User who made the order
    }
}