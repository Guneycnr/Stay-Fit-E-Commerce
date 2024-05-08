using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Sipariş eklemek için oluşturduğum sınıf
public class OrderItem
{
    public int ProductId { get; set; } // Ürün kimliği
    public int Quantity { get; set; }  // Ürün miktarı
    public decimal UnitPrice { get; set; } // Ürün birim fiyatı
}