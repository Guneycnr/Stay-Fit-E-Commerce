using BitirmeProjesi.Presentation.Areas.Account.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BitirmeProjesi.Data.Entities.JsonEntities;
using System.Text.Json.Serialization;

namespace BitirmeProjesi.Presentation.Areas.Account.Controllers;

[Area("Account")]
public class UserController : Controller
{
    private readonly HttpClient _httpClient;

    public UserController( IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api"); // Api bağlantısı
    }

    // Kayıt Sayfası
    public IActionResult Register()
    {
        return View();
    }

    // Kayıt İşlemi
    [HttpPost]
    public async Task<IActionResult> PostRegister(RegistrationRequest request)
    {
        var apiUrl = "http://localhost:5118/api/account/register"; // API'nin URL'si

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("SignIn");
        }
        else
        {
            // Hata durumunda hata mesajını döndür
            ViewBag.ErrorMessage = "Kayıt işlemi başarısız oldu.";
            return View("Register", request);
        }
    }

    // Giriş Sayfası
    public IActionResult SignIn()
    {
        return View();
    }

    //Giriş İşlemleri
    [HttpPost]
    public async Task<IActionResult> PostSignIn(AuthRequest request)
    {
        var apiUrl = "http://localhost:5118/api/account/login"; // Giriş API'sinin URL'si

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, options);

            if (authResponse != null && !string.IsNullOrEmpty(authResponse.token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.token);

                // Başarılı giriş durumunda kullanıcı bilgilerini sakla
                var appUser = new BitirmeProjesi.Data.Entities.JsonEntities.AppUser
                {
                    Id = authResponse.id,
                    UserName = authResponse.username,
                    Email = authResponse.email,
                    Role = authResponse.Role, // Rol bilgisini API'den al
                };

                // Kullanıcı bilgilerini oturumda sakla
                HttpContext.Session.SetString("User", JsonSerializer.Serialize(appUser));
            }
            return RedirectToRoute(new { action = "Index", controller = "Home", area = "" });
        }
        else
        {
            ViewBag.ErrorMessage = "Giriş yapılamadı. Kullanıcı adı veya şifre hatalı.";
            return View("SignIn", request); // Hata durumunda SignIn sayfasını tekrar göster
        }
    }


    // Çıkış işlemi
    public async Task<IActionResult> SignOut()
    {
        // ViewBag içindeki kullanıcı bilgilerini temizle
        ViewBag.User = null;

        // Oturum bilgilerini temizle
        HttpContext.Session.Clear();

        // Çıkış işlemi tamamlandıktan sonra yönlendirme yapılabilir
        return RedirectToRoute(new { action = "Index", controller = "Home", area = "" });
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}