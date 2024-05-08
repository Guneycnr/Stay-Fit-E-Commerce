using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Apiden gelen json veriyi karşılamak için oluşturduğum Order sınıfı
public class Order
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("adres")]
    public string Adres { get; set; } = string.Empty;

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }


    // İlişkiler 
    public List<OrderItem> Items { get; set; } // Sipariş öğeleri

    [JsonPropertyName("appUserId")]
    public string AppUserId { get; set; }

    [JsonPropertyName("shipperId")]
    public int ShipperId { get; set; }
}
