using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; }   
        
        [Required]
        public string Username { get; set; }
        
        [Required,DataType(DataType.EmailAddress),EmailAddress]
        public string Email { get; set; }

        [Required,DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }   
    }
}