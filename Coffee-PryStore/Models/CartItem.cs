namespace Coffee_PryStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }


      //  public virtual ICollection<Table> Table { get; set; } = new List<Table>();
    }

}
