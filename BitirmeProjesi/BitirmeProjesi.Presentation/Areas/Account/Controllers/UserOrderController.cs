using BitirmeProjesi.Data.Entities.JsonEntities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BitirmeProjesi.Presentation.Areas.Account.Controllers;

[Area("Account")]
public class UserOrderController : Controller
{
    private readonly HttpClient _httpClient;

    public UserOrderController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Kullanıcıya ait siparişleri listeler
    [TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "User" })]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Oturumdan kullanıcı kimliğini al
        var appUser = await GetAppUserFromSessionAsync();
        if (appUser == null)
        {
            return RedirectToAction("SignIn", "Account");
        }

        var apiUrl = "http://localhost:5118/api/orders"; // Uygulamada çok az sipariş olduğu için hepsini alıp sorguyu öyle yapıyoruz ilerleyen döndemde apiye sadece kullanıcıya ait siparişleri al olarak performans iyileştirmesi yapılabilir

        // API'den gelen tüm siparişler
        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "API error - failed to retrieve orders.");
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        // Tüm siparişleri JSON'dan listeye dönüştürme işlemi
        var allOrders = JsonConvert.DeserializeObject<List<Order>>(responseContent);

        if (allOrders == null)
        {
            return StatusCode(500, "Siparişler Listelenemedi.");
        }

        // Kullanıcıya ait siparişleri filtreleme
        var userOrders = allOrders.Where(order => order.UserName == appUser.UserName).ToList();

        return View(userOrders); // Filtrelenmiş siparişleri View'a geçir
    }

    private async Task<AppUser?> GetAppUserFromSessionAsync()
    {
        var userJson = HttpContext.Session.GetString("User"); // Session'da "User" anahtarı ile saklanan string'i al.
        if (string.IsNullOrEmpty(userJson) )// Eğer alınan değer boş veya null ise,
        {
            return null; // null döndür ve işlemi sonlandır.
        }

        return JsonConvert.DeserializeObject<AppUser>(userJson); // JSON string'ini AppUser nesnesine dönüştür ve döndür.
    }
}
