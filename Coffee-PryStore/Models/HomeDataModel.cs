using System.ComponentModel.DataAnnotations;

namespace Coffee_PryStore.Models
{
    public class HomeDataModel
    {
        
        public int Id { get; set; }
        public required string Title { get; set; }
       public required string Author { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }
        
     
    }
}

