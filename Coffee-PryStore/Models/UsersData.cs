using System.ComponentModel.DataAnnotations;

namespace Coffee_PryStore.Models
{
    public class UsersData
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string UserPassword { get; set; }


        public string UserPhone { get; set; }
        public string UserName { get; set; }
    }
}
