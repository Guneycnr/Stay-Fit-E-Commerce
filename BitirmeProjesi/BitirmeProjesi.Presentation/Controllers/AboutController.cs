using Microsoft.AspNetCore.Mvc;

namespace BitirmeProjesi.Presentation.Controllers;

public class AboutController : Controller
{
    public IActionResult About()
    {
        return View();
    }
}
