using BitirmeProjesi.Data.Entities.JsonEntities;
using System.Text.Json.Serialization;

namespace BitirmeProjesi.Presentation.Areas.Account.Models;
public class AuthResponse
{
    // Giriş yapan kullanıcıya döndüğümüz bilgiler
    public string id { get; set; }
    public string username { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    [JsonPropertyName("role")]
    public Role Role { get; set; }
    public string token { get; set; } = string.Empty;
}
