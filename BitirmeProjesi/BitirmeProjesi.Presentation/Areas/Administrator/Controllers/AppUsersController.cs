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
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.AspNetCore.Authorization;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;


namespace BitirmeProjesi.Presentation.Areas.Administrator.Controllers;

[TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
[Area("Administrator")]
public class AppUsersController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public AppUsersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/AppUsers
    public async Task<IActionResult> Index()
    {
        var userJson = await _httpClient.GetStringAsync("appuser");

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        var userList = JsonSerializer.Deserialize<List<AppUser>>(userJson, options);

        return View(userList);

    }

    // GET: Administrator/AppUsers/Details/Id
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            // API'den ilgili JSON verisini al
            var userJson = await _httpClient.GetStringAsync($"appuser/{id}");

            // JSON verisini AppUser nesnesine dönüştür
            var userDetail = Deserialize<AppUser>(userJson);

            // Detaylarını View'e gönder
            return View(userDetail);
        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: Administrator/AppUsers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Administrator/AppUsers/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("UserName,Email,PhoneNumber,Role,Adres,City,Country")] AppUser appUser)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/appuser"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(appUser), Encoding.UTF8, "application/json");

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

    // GET: Administrator/AppUsers/Edit/Id
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"appuser/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var user = Deserialize<AppUser>(jsonString);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
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

    // POST: Administrator/AppUsers/Edit/Id
    [HttpPost]
    public async Task<IActionResult> Edit(string id, AppUser appUser)
    {
        if (id != appUser.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var apiUrl = $"http://localhost:5118/api/appuser/{id}"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(appUser), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bilgilerini güncellerken bir hata oluştu. Lütfen daha sonra tekrar deneyin."); // Hata Mesajı
                return View(appUser);
            }
        }
        return View(appUser);
    }


    // POST: Administrator/AppUsers/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"appuser/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
                ModelState.AddModelError("", "Kullanıcı silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return RedirectToAction(nameof(Delete), new { id }); // silme işlemi başarısız olursa delete sayfasına geri dön
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda loglama veya uygun işlemler yapılabilir
            Console.WriteLine("Hata oluştu: " + ex.Message);
            ModelState.AddModelError("", "Kullanıcı silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return RedirectToAction(nameof(Delete), new { id }); // silme işlemi başarısız olursa delete sayfasına geri dön
        }
    }


    private T Deserialize<T>(string json)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        return JsonSerializer.Deserialize<T>(json, options)!;
    }
}
