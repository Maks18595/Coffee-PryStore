using Coffee_PryStore.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



    namespace Coffee_PryStore.Models
    {
        public class Table
        {
        public int CofId { get; set; } // Не забувайте про ID, якщо він є у вашій базі даних

        [Required]
        public string CofName { get; set; }

        [Required]
        public string CofCateg { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більшою за 0")]
        public decimal CofPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути меншою за 0")]
        public int CofAmount { get; set; }

        [Required]
        public DateTime CofDuration { get; set; }

        public string ImagePath { get; set; }

        // Навігаційна властивість
        public virtual CategoryTable Category { get; set; }
        }
    }






