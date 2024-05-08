using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesi.Data.Entities.JsonEntities;
using System.Text.Json;
using System.Text;
using System.Net;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;

namespace BitirmeProjesi.Presentation.Areas.Administrator.Controllers;

[TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
[Area("Administrator")]
public class ContactsController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public ContactsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/Contacts
    public async Task<IActionResult> Index()
    {
        var contactJson = await _httpClient.GetStringAsync("contact");

        var contact = Deserialize<List<Contact>>(contactJson);

        return View(contact);
    }

    // GET: Administrator/Contacts/Details/Id
    public async Task<IActionResult> Details(int? id)
    {
        try
        {
            // API'den ilgili JSON verisini al
            var contactJson = await _httpClient.GetStringAsync($"contact/{id}");

            // JSON verisini Contact nesnesine dönüştür
            var contactDetail = Deserialize<Contact>(contactJson);

            // Ürün detaylarını View'e gönder
            return View(contactDetail);
        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: Administrator/Contacts/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Administrator/Contacts/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Email,Message")] Contact contact)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/contact"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(contact), Encoding.UTF8, "application/json");

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

    // GET: Administrator/Contacts/Edit/Id
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"contact/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<Contact>(jsonString);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
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

    // POST: Administrator/Contacts/Edit/Id
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Contact contact)
    {
        if (id != contact.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"contact/{id}", contact);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Contact bilgilerini güncellerken bir hata oluştu. Lütfen daha sonra tekrar deneyin."); // Hata Mesajı
                return View(contact);
            }
        }
        return View(contact);
    }

    // GET: Administrator/Contacts/Delete/Id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"contact/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<Contact>(jsonString);
            return View(contact);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Administrator/Contacts/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"contact/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
            ModelState.AddModelError("", "Mesaj silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(); // veya hata sayfasına yönlendirme yapılabilir
        }
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
