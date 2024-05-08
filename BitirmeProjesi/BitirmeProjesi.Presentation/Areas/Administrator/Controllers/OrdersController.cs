using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesi.Data.Entities.JsonEntities;
using BitirmeProjesi.Presentation.Areas.Administrator;
using System.Text.Json;
using System.Text;
using BitirmeProjesi.Presentation.Areas.Account.Controllers;

namespace BitirmeProjesi.Presentation.Areas.Administrator.Controllers;

[TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
[Area("Administrator")]
public class OrdersController : Controller
{
    private readonly HttpClient _httpClient;

    // Apiyi controller üzerine taşıyoruz
    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("comp-api");
    }

    // GET: Administrator/Orders
    public async Task<IActionResult> Index()
    {
        var orderJson = await _httpClient.GetStringAsync("orders");

        var order = Deserialize<List<Order>>(orderJson);

        return View(order);
    }

    // GET: Administrator/Orders/Details/Id
    public async Task<IActionResult> Details(int? id)
    {
        try
        {
            // API'den ilgili siparişin JSON verisini al
            var orderJson = await _httpClient.GetStringAsync($"orders/{id}");

            // JSON verisini Order nesnesine dönüştür
            var orderDetail = Deserialize<Order>(orderJson);

            // Sipariş detaylarını View'e gönder
            return View(orderDetail);
        }
        catch (Exception ex)
        {
            // Hata durumunda hata sayfasına yönlendir
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: Administrator/Orders/Delete/Id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"orders/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<Order>(jsonString);
            return View(order);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Administrator/Orders/Delete/Id
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"orders/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Eğer işlem başarısızsa, kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
            ModelState.AddModelError("", "Order silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(); // veya hata sayfasına yönlendirme yapılabilir
        }
    }

    private T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
