using System.ComponentModel.DataAnnotations;

namespace api.Entities
{
    public class AppUser
    {   [Key]
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; }   
        
        [Required]
        public string Username { get; set; }
        
        [Required,DataType(DataType.EmailAddress),EmailAddress]
        public string Email { get; set; }

        [Required,DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        
        [Required]
        public byte[] PasswordHash { get; set; }
        
        [Required]
        public byte[] PasswordSalt { get; set; }
    }
}