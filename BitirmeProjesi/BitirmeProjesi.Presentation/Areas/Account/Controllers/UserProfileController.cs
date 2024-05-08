using BitirmeProjesi.Data.Entities.JsonEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Owin.BuilderProperties;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitirmeProjesi.Presentation.Areas.Account.Controllers;

[Area("Account")]
public class UserProfileController : Controller
{
    private readonly HttpClient _httpClient;

    public UserProfileController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Profile()
    {
        // Oturumdan AppUser'ı al
        var userJson = HttpContext.Session.GetString("User");

        if (string.IsNullOrEmpty(userJson))
        {
            return RedirectToAction("SignIn", "Account"); // kullanıcı gitiş yapmadan işlem yapmaya çalışırsa giriş sayfasına yönlendir
        }

        var appUser = JsonConvert.DeserializeObject<AppUser>(userJson);

        if (appUser == null || string.IsNullOrEmpty(appUser.Id))
        {
            return RedirectToAction("SignIn", "Account"); // AppUser boş ise giriş sayfasına yönlendir
        }

        // Kullanıcı ID'sini kullanarak API'ye istek gönder
        var apiUrl = $"http://localhost:5118/api/appuser/{appUser.Id}";
        
        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "API error");
        }

        var responseContent = await response.Content.ReadAsStringAsync();

            var userProfile = JsonConvert.DeserializeObject<AppUser>(responseContent);
            return View(userProfile); // Alınan kullanıcı bilgilerini viewa gönder
    }

    // Kullnıcının Bilgilerini Alıp Label İçinde Göster
    public async Task<IActionResult> UpdateProfile()
    {
        // Oturumdan AppUser'ı al
        var appUser = await GetAppUserFromSessionAsync();
        if (appUser == null)
        {
            return RedirectToAction("SignIn", "Account");
        }

        // Kullanıcı profilini al ve UpdateProfile görünümüne ilet
        var userProfile = await GetUserProfileAsync(appUser.Id);
        if (userProfile == null)
        {
            return StatusCode(500, "Failed to retrieve user profile.");
        }

        return View(userProfile); // Profil verilerini burada View'a geçir
    }

    // Label içinde kullanıcının girdiği veri ile kullanıcı bilgilerni güncelle
    [HttpPost]
    public async Task<IActionResult> EditProfile(AppUser updatedUser)
    {
        if (!ModelState.IsValid)
        {
            // Eğer model geçerli değilse, hataları görmek için formu tekrar göster
            return View(updatedUser);
        }

        // API'ye PUT isteği ile güncellenmiş kullanıcı verilerini gönder
        var apiUrl = $"http://localhost:5118/api/appuser/{updatedUser.Id}";
        var content = new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(apiUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            // API yanıtı başarısız olursa hata mesajı döndür
            return StatusCode((int)response.StatusCode, "Failed to update user profile.");
        }

        // Güncelleme başarılı olursa kullanıcıyı profiline yönlendir
        return RedirectToAction("Profile");
    }

    private async Task<AppUser?> GetAppUserFromSessionAsync() // Session'dan AppUser nesnesini almak için asenkron bir metot.
    {
        var userJson = HttpContext.Session.GetString("User"); // Session'da "User" anahtarı ile saklanan veriyi al.

        if (string.IsNullOrEmpty(userJson)) // Eğer alınan veri boş veya null ise,
        {
            return null; // Metodun sonucu olarak null döndür.
        }

        return JsonConvert.DeserializeObject<AppUser>(userJson); // JSON formatındaki veriyi AppUser nesnesine dönüştür ve döndür.
    }

    private async Task<AppUser?> GetUserProfileAsync(string userId) // Belirtilen kullanıcı ID'sine göre kullanıcı profili verisini almak için asenkron metot.
    {
        var apiUrl = $"http://localhost:5118/api/appuser/{userId}"; // API'nin URL'si, kullanıcı ID'siyle birlikte.

        var response = await _httpClient.GetAsync(apiUrl); // HTTP GET isteği gönder ve yanıtı al.

        if (!response.IsSuccessStatusCode) // Eğer yanıt başarılı değilse,
        {
            return null; // null döndür ve metodu sonlandır.
        }

        var responseContent = await response.Content.ReadAsStringAsync(); // Yanıt içeriğini string olarak al.
        return JsonConvert.DeserializeObject<AppUser>(responseContent); // İçeriği JSON'dan AppUser nesnesine dönüştür ve döndür.
    }
}


