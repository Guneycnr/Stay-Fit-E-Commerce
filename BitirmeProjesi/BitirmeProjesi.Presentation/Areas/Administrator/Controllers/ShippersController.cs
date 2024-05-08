using BitirmeProjesi.Data.Entities.JsonEntities;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
[Area("Administrator")]
public class ShippersController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public ShippersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/Shippers
    public async Task<IActionResult> Index()
    {
        var shipperJson = await _httpClient.GetStringAsync("shippers");

        var shipper = Deserialize<List<Shipper>>(shipperJson);

        return View(shipper);
    }

    // GET: Administrator/Shippers/Details/Id
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            // API'den ilgili ürünün JSON verisini al
            var shipperJson = await _httpClient.GetStringAsync($"shippers/{id}");

            // JSON verisini Product nesnesine dönüştür
            var shipperDetail = Deserialize<Shipper>(shipperJson);

            // Ürün detaylarını View'e gönder
            return View(shipperDetail);
        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: Administrator/Shippers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Administrator/Shippers/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("CompanyName,Phone")] Shipper shipper)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/shippers"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(shipper), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Shipper ekleme işlemi başarılı ise admin ana sayfayasına geri döner
            }

            else
            {
                // Ekleme işlemi başarısız olursa hata döner
                ModelState.AddModelError("", "Shipper eklerken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View();
            }
        }
        else
        {
            return View();
        }
    }


    // GET: Administrator/Shippers/Edit/Id
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"shippers/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var shipper = JsonSerializer.Deserialize<Shipper>(jsonString);
            if (shipper == null)
            {
                return NotFound();
            }
            return View(shipper);
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

    // POST: Administrator/Shippers/Edit/Id
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Shipper shipper)
    {
        if (id != shipper.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"shippers/{id}", shipper);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Shipper bilgilerini güncellerken bir hata oluştu. Lütfen daha sonra tekrar deneyin."); // Hata Mesajı
                return View(shipper);
            }
        }
        return View(shipper);
    }


    // GET: Administrator/Shippers/Delete/Id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"shippers/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var shipper = JsonSerializer.Deserialize<Shipper>(jsonString);
            return View(shipper);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Administrator/Shippers/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"shippers/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
            ModelState.AddModelError("", "Shipper silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(); // veya hata sayfasına yönlendirme yapılabilir
        }
    }


    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}