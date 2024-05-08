using BitirmeProjesi.Presentation.Areas.Account.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitirmeProjesi.Presentation.Areas.Administrator.Controllers;

public class HomeController : Controller
{
    [TypeFilter(typeof(RoleAuthorizationFilter), Arguments = new object[] { "Admin" })] // Sadece admin rolüne sahip olanlar girebilir
    [Area("Administrator")]
    public IActionResult Index()
    {
        return View();
    }
}
