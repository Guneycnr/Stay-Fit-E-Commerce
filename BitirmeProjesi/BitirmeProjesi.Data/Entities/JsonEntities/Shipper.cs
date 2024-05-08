using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Apiden gelen json veriyi karşılamak için oluşturduğum Shipper sınıfı
public class Shipper
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
}
