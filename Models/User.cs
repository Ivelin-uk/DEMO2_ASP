using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcApp.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Моля, въведете име.")]
        [Column("FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Моля, въведете фамилия.")]
        [Column("LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Моля, въведете имейл.")]
        [EmailAddress(ErrorMessage = "Моля, въведете валиден имейл.")]
        [Column("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Моля, въведете парола.")]
        [MinLength(6, ErrorMessage = "Паролата трябва да бъде поне 6 символа.")]
        [RegularExpression(@"^(?=.*[A-Z]).+$", ErrorMessage = "Паролата трябва да съдържа поне една главна буква.")]
        [Column("Password")]
        public string Password { get; set; }

        [Required]
        [Column("Role")]
        public string Role { get; set; } 
    }
}