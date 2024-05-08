using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Sepette ürün görmek için yazdığım sınıf
public class Sepet
{
    public List<Urun> Urunler { get; set; } = new List<Urun>();

    public void Ekle(Urun urun)
    {
        // Sepette ürün kontrol 
        var existingUrun = Urunler.FirstOrDefault(x => x.Id == urun.Id);

        if (existingUrun != null)
        {
            // Ürün zaten sepetteyse, miktarını artır
            existingUrun.Quantity += 1;
        }
        else
        {
            // Yeni ürünü varsayılan miktar ile (1) sepete ekle
            urun.Quantity = 1; // Varsayılan miktar
            Urunler.Add(urun);
        }
    }

}
    public class Urun
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("photoUrl")]
        public string Photourl { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }