using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesi.Data.Entities.JsonEntities;
using System.Text;
using System.Text.Json;
using System.Net;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;

namespace BitirmeProjesi.Presentation.Areas.Administrator.Controllers;

[TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
[Area("Administrator")]
public class CategoriesController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public CategoriesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/Categories
    public async Task<IActionResult> Index()
    {
        var categoryJson = await _httpClient.GetStringAsync("categories");

        var category = Deserialize<List<Category>>(categoryJson);

        return View(category);
    }

    // GET: Administrator/Categories/Details/Id
    public async Task<IActionResult> Details(int? id)
    {
        try
        {
            // API'den ilgili JSON verisini al
            var categoryJson = await _httpClient.GetStringAsync($"categories/{id}");

            // JSON verisini Category nesnesine dönüştür
            var caategoryDetail = Deserialize<Category>(categoryJson);

            // Detaylarını View'e gönder
            return View(caategoryDetail);
        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: Administrator/Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Administrator/Categories/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("CategoryName")] Category category)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/categories"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // İşlem başarılı ise admin ana sayfayasına geri döner
            }

            else
            {
                // İşlem başarısız olursa hata döner
                ModelState.AddModelError("", "Mesajı eklerken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View();
            }
        }
        else
        {
            return View();
        }
    }

    // GET: Administrator/Categories/Edit/Id
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"categories/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(jsonString);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
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

    // POST: Administrator/Categories/Edit/Id
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"categories/{id}", category);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Categories bilgilerini güncellerken bir hata oluştu. Lütfen daha sonra tekrar deneyin."); // Hata Mesajı
                return View(category);
            }
        }
        return View(category);
    }

    // GET: Administrator/Categories/Delete/Id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"categories/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(jsonString);
            return View(category);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Administrator/Categories/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
         HttpResponseMessage response = await _httpClient.DeleteAsync($"categories/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
            ModelState.AddModelError("", "Veri silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(); // veya hata sayfasına yönlendirme yapılabilir
        }
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
