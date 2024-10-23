using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Coffee_PryStore.Models
{
    public class CategoryTable
    {
        [Key]
        [Required] 
        public string CategID { get; set; } = string.Empty;
        [Required]
        public string CategName { get; set; } = string.Empty;

        public string CategDescript { get; set; } = string.Empty;

        public virtual ICollection<Table> Table { get; set; } = new List<Table>();

    }
}

