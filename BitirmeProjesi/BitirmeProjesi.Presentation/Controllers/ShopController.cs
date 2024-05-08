using BitirmeProjesi.Data.Entities.JsonEntities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BitirmeProjesi.Presentation.Controllers;

public class ShopController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public ShopController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // İstekte bulunacağımız controlleri çağırıyoruz
    public async Task<IActionResult> Index()
    {
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    public async Task<IActionResult> compression()
    {
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    public async Task<IActionResult> hoodie()
    {
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    public async Task<IActionResult> tshirt()
    {
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    public async Task<IActionResult> joger()
    {
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}

