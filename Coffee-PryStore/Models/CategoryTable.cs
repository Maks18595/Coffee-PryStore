using System.ComponentModel.DataAnnotations;
    using Coffee_PryStore.Models;
   


    namespace Coffee_PryStore.Models
    {
        public class CategoryTable
        {
            [Key]
            public string CategID { get; set; }  // Це буде первинний ключ у таблиці категорій

            public  string CategName { get; set; }

            public  string CategDescript { get; set; }

            // Навігаційна властивість для зв'язку з продуктами
            public virtual  ICollection<Table> Table { get; set; }
        }
    }



