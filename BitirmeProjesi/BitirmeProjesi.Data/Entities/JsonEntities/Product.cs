using Core.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Apiden gelen json veriyi karşılamak için oluşturduğum Product sınıfı
public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("usedMaterial")]
    public string UsedMaterial { get; set; } = string.Empty;

    [JsonPropertyName("commentLine")]
    public string CommentLine { get; set; } = string.Empty;

    [JsonPropertyName("photoUrl")]
    public string PhotoUrl { get; set; } = string.Empty;

    [JsonPropertyName("midPhoto")]
    public string MidPhoto { get; set; } = string.Empty;

    [JsonPropertyName("lastPhoto")]
    public string LastPhoto { get; set; } = string.Empty;

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("unitStock")]
    public int UnitStock { get; set; }


    // İlişkiler

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }
}
