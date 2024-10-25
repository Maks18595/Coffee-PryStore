using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffee_PryStore.Models
{
    public class Basket
    {
        [Key]
        public int CartItemId { get; set; } 

        public int ProductId { get; set; } 

        public int CofId { get; set; } 

        public int Quantity { get; set; }


        [ForeignKey("CofId")]
        public virtual Table Cof { get; set; }

    }
}
