using Coffee_PryStore.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffee_PryStore.Models
{
    public class Table
    {
        [Key]
        public int CofId { get; set; }

        public string CofName { get; set; } = string.Empty;

        [ForeignKey("Category")]
        public string CofCateg { get; set; } = string.Empty;

        public decimal CofPrice { get; set; }

        public int CofAmount { get; set; }

        public DateTime CofDuration { get; set; }

        public byte[]? ImageData { get; set; }

        public string? Description { get; set; }

       
    }
}






