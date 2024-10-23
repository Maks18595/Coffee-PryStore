using System.ComponentModel.DataAnnotations;

namespace Coffee_PryStore.Models
{
    public class UsersData
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserEmail { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Password is required")]
        public string UserPassword { get; set; } = string.Empty; 

        public string UserPhone { get; set; } = string.Empty; 
        public string UserName { get; set; } = string.Empty; 
    }
}
