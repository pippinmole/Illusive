using System.ComponentModel.DataAnnotations;

namespace Illusive.Illusive.Database.Models {
    public class SignupDataForm {
            
        [Required, MaxLength(15)]
        public string Username { get; set; }
            
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
            
        [Required, StringLength(32), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}