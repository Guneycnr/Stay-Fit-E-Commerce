using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Presentation.Areas.Account.Models;

public class AuthRequest
{
    // Kullanıcının giriş yapması için doldurması gereken bilgiler 

    [Required]
    public string email { get; set; } = string.Empty;

    [Required]
    public string password { get; set; } = string.Empty;
}
