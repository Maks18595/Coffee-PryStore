using System.ComponentModel.DataAnnotations;

namespace Coffee_PryStore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public  string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public  string Password { get; set; }

        public  string Role { get; set; }
    }



}
