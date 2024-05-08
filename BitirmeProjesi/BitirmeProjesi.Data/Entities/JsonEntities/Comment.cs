using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Apiden gelen json veriyi karşılamak için oluşturduğum Comment sınıfı
public class Comment
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("commentLine")]
    public string CommentLine { get; set; } = string.Empty;


    // İlişkiler 

    [JsonPropertyName("appUserId")]
    public string AppUserId { get; set; } 
    
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

}
