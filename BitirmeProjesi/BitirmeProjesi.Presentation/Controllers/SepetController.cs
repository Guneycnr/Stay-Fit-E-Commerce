using BitirmeProjesi.Data.Entities.JsonEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BitirmeProjesi.Business.Extensions;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace BitirmeProjesi.Presentation.Controllers;

public class SepetController : Controller
{
    private readonly ProductService _productService;
    private readonly ILogger<SepetController> _logger;
    private readonly HttpClient _httpClient;

    public SepetController(HttpClient httpClient, ILogger<SepetController> logger,ProductService productService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _productService = productService;
    }

    // Sepeti Gösterir
    public IActionResult Index()
    {
        var sepet = HttpContext.Session.Get<Sepet>("sepet") ?? new Sepet();
        return View(sepet);
    }


    // Sepete Ekleme Yapar
    public async Task<IActionResult> Ekle(int urunId)
    {
        // Sepeti oturumdan alın, yoksa yeni bir sepet oluşturun
        var sepet = HttpContext.Session.Get<Sepet>("sepet") ?? new Sepet();

        // Ürünü kimliği ile alın
        var urun = await _productService.GetProductByIdAsync(urunId);

        if (urun != null)
        {
            sepet.Ekle(urun); // Sepete ekle, miktarı kontrol edecek
            HttpContext.Session.Set("sepet", sepet); // Sepeti oturuma geri koy
        }

        return RedirectToAction("Index", "Sepet"); // Sepet sayfasına yönlendir
    }

    // Ürün adedini günceller
    [HttpPost]
    public IActionResult UpdateQuantity(int urunId, int quantity)
    {
        var sepet = HttpContext.Session.Get<Sepet>("sepet");
        if (sepet == null)
        {
            return RedirectToAction("Index", "Sepet"); // Sepet yoksa ana sayfaya dön
        }

        var urun = sepet.Urunler.FirstOrDefault(x => x.Id == urunId);
        if (urun != null)
        {
            urun.Quantity = quantity; // Miktarı güncelle
            HttpContext.Session.Set("sepet", sepet); // Sepeti oturuma güncelle
        }

        return RedirectToAction("Index", "Sepet"); // Sepet sayfasını yeniden yükle
    }

    //Sepetten Ürün Siler
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Kaldir(int urunId)
    {
        var sepet = HttpContext.Session.Get<Sepet>("sepet");
        if (sepet != null)
        {
            var urun = sepet.Urunler.FirstOrDefault(u => u.Id == urunId);
            if (urun != null)
            {
                sepet.Urunler.Remove(urun);
                HttpContext.Session.Set("sepet", sepet);
            }
        }
        return RedirectToAction("Index"); // Sepet sayfasına geri dön
    }


    // Ödemeye Gider
    [TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "User" })]
    public IActionResult CheckOut()
    {
        return View();
    }


    // Siparişi API'ye gönderip sepeti temizler
    [HttpPost]
    public async Task<IActionResult> SiparisVer()
    {
        var sepet = HttpContext.Session.Get<Sepet>("sepet");

        if (sepet == null || sepet.Urunler.Count == 0)
        {
            return RedirectToAction("Index", "Sepet");
        }

        // Oturumdaki kullanıcı bilgilerini alın
        var userJson = HttpContext.Session.GetString("User");
        if (string.IsNullOrEmpty(userJson))
        {
            return RedirectToAction("SignIn"); // Kullanıcı oturum açmamışsa giriş sayfasına yönlendirin
        }

        var appUser = JsonConvert.DeserializeObject<AppUser>(userJson);

        var order = new Order
        {
            OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local), // UTC tarihini yerel saat dilimine çevir
            AppUserId = appUser.Id,
            UserName = appUser.UserName,
            Adres = appUser.Adres,
            ShipperId = 1,
            TotalPrice = sepet.Urunler.Sum(u => u.UnitPrice * u.Quantity),
            Items = sepet.Urunler.Select(u => new OrderItem
            {
                ProductId = u.Id,
                Quantity = u.Quantity,
                UnitPrice = u.UnitPrice,
            }).ToList(),
        };

        var json = JsonConvert.SerializeObject(order, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("http://localhost:5118/api/orders", content);

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Remove("sepet"); // Sepeti temizle
                return RedirectToAction("Basari");
            }
            else
            {
                _logger.LogError($"API hatası: {response.StatusCode} - {response.ReasonPhrase}");
                return RedirectToAction("Index", "Sepet"); // Hata durumunda sepet sayfasına dön
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API'ye sipariş gönderilirken hata oluştu");
            return RedirectToAction("Index", "Sepet"); // Hata durumunda sepet sayfasına dön
        }
    }

    // Ödeme başarılı ise
    public IActionResult Basari()
    {
        return View();
    }

}
