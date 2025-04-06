using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Името е задължително")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Фамилията е задължителна")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Имейлът е задължителен")]
    [EmailAddress(ErrorMessage = "Невалиден имейл")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Паролата е задължителна")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
