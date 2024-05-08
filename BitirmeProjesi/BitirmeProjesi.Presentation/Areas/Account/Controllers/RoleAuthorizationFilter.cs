using BitirmeProjesi.Data.Entities.JsonEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace BitirmeProjesi.Presentation.Areas.Account.Controllers;

// Özel bir Action Filter ile rol kontrolü
public class RoleAuthorizationFilter : IActionFilter
{
    private readonly string _requiredRole;

    public RoleAuthorizationFilter(string requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userSession = context.HttpContext.Session.GetString("User");
        if (string.IsNullOrEmpty(userSession))
        {
            // Kullanıcı oturum açmadan işlem yapmaya çalışırsa giriş sayfasına yönlendir
            context.Result = new RedirectToActionResult("SignIn", "User", new { area = "Account" }); 
            return;
        }

        var appUser = JsonSerializer.Deserialize<AppUser>(userSession);
        if (appUser == null || appUser.Role.ToString() != _requiredRole)
        {
            context.Result = new ForbidResult(); // Erişim yasak
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Bu bölüm gerekli değil
    }
}