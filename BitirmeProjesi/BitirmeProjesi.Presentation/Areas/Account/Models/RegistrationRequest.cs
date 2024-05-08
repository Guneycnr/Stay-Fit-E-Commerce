using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Presentation.Areas.Account.Models;

public class RegistrationRequest
{
    // Kullanıcının kayıt olması için doldurması gereken bilgiler
    [Required]
    [EmailAddress]
    public string email { get; set; } = string.Empty;

    [Required]
    public string username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; } = string.Empty;

}
