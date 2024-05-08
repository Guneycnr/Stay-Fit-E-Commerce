using BitirmeProjesi.Data.Entities.JsonEntities;
using BitirmeProjesi.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Text.Json;


namespace BitirmeProjesi.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // İstekte bulunacağımız veriyi çağırıyoruz
    public async Task<IActionResult> Index()
    {        
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            // API'den ilgili ürünün JSON verisini al
            var productJson = await _httpClient.GetStringAsync($"products/{id}");

            // JSON verisini Product nesnesine dönüştür
            var productDetail = Deserialize<Product>(productJson);

            // Ürün detaylarını View'e gönder
            return View(productDetail);

        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
