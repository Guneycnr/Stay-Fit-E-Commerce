using BitirmeProjesi.Data.Entities.JsonEntities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BitirmeProjesi.Presentation.Controllers;

public class ContactController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public ContactController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Post(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            var apiUrl = "http://localhost:5118/api/contact"; // API'nin URL'si

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Contact"); // Mesaj gönderme işlemi başarılı ise contact sayfasına geri döner
            }

            else
            {
                // Mesaj gönderme işlemi başarısız olursa hata döner
                ModelState.AddModelError("", "Mesajınız gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin."); 
                return View();
            }
        }
        else
        {
            return View();
        }
    }
    public class ContactViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }

}

