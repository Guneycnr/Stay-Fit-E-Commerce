using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesi.Data.Entities.JsonEntities;
using BitirmeProjesi.Presentation.Areas.Administrator;
using System.Text.Json;
using System.Text;
using System.Net;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;

namespace BitirmeProjesi.Presentation.Areas.Administrator.Controllers;

[TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
[Area("Administrator")]
public class ProductsController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public ProductsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/Products
    public async Task<IActionResult> Index()
    {
        var productJson = await _httpClient.GetStringAsync("products");

        var product = Deserialize<List<Product>>(productJson);

        return View(product);
    }

    // GET: Administrator/Products/Details/Id
    public async Task<IActionResult> Details(int id)
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

    // GET: Administrator/Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Administrator/Products/Create       
    [HttpPost]
    public async Task<IActionResult> Create([Bind("ProductName,Description,UsedMaterial,PhotoUrl,MidPhoto,LastPhoto,UnitPrice,UnitStock,CategoryId")]Product product)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/products"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Product ekleme işlemi başarılı ise admin ana sayfayasına geri döner
            }

            else
            {
                // Ekleme işlemi başarısız olursa hata döner
                ModelState.AddModelError("", "Product eklerken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View();
            }
        }
        else
        {
            return View();
        }
    }

    // GET: Administrator/Products/Edit/Id
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"products/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var productt = JsonSerializer.Deserialize<Product>(jsonString);
            if (productt == null)
            {
                return NotFound();
            }
            return View(productt);
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        else
        {
            // Hata oluşursa
            ModelState.AddModelError("", "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View();
        }
    }

    // POST: Administrator/Products/Edit/Id
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"products/{id}", product);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Product bilgilerini güncellerken bir hata oluştu. Lütfen daha sonra tekrar deneyin."); // Hata Mesajı
                return View(product);
            }
        }
        return View(product);
    }

    // GET: Administrator/Products/Delete/Id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"products/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var productt = JsonSerializer.Deserialize<Product>(jsonString);
            return View(productt);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Administrator/Products/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"products/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
            ModelState.AddModelError("", "Product silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(); // veya hata sayfasına yönlendirme yapılabilir
        }
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
