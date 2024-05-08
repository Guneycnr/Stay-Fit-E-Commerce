using BitirmeProjesi.Data.Entities.JsonEntities;
using Newtonsoft.Json;

namespace BitirmeProjesi.Presentation.Controllers;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Urun> GetProductByIdAsync(int productId)
    {
        var response = await _httpClient.GetAsync($"http://localhost:5118/api/products/{productId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonConvert.DeserializeObject<Urun>(content); // Ürün bilgilerini deserialize et
        return product;
    }
}