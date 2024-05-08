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
public class CommentsController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public CommentsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/Comments
    public async Task<IActionResult> Index()
    {
        var commentJson = await _httpClient.GetStringAsync("comments");

        var comment = Deserialize<List<Comment>>(commentJson);

        return View(comment);
    }

    // GET: Administrator/Comments/Details/Id
    public async Task<IActionResult> Details(int? id)
    {
        try
        {
            // API'den ilgili JSON verisini al
            var commentJson = await _httpClient.GetStringAsync($"comments/{id}");

            // JSON verisini Comment nesnesine dönüştür
            var commentDetail = Deserialize<Comment>(commentJson);

            // Ürün detaylarını View'e gönder
            return View(commentDetail);
        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: Administrator/Comments/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Administrator/Comments/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("ProductId,AppUserId,CommentLine")] Comment comment)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/comments"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(comment), Encoding.UTF8, "application/json");

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


    // GET: Administrator/Comments/Delete/Id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"comments/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var comment = JsonSerializer.Deserialize<Comment>(jsonString);
            return View(comment);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Administrator/Comments/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"comments/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
            ModelState.AddModelError("", "Yorum silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(); // veya hata sayfasına yönlendirme yapılabilir
        }
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
