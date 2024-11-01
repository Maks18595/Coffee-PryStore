using Coffee_PryStore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [ForeignKey("Table")]
    public int CofId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Table Table { get; set; } = null!;
}