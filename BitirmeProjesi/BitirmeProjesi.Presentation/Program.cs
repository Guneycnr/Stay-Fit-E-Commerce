using BitirmeProjesi.Presentation.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Apiden istekte bulunuyoruz
builder.Services.AddHttpClient("comp-api", c =>
{
    c.BaseAddress = new Uri("http://localhost:5118/api/");
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "http://localhost:5118"; // Tokenin sa�lay�c�s�n�n URL'si
    options.Audience = "comp-api"; // API kayna��n�n ad�
    options.RequireHttpsMetadata = false; // Geli�tirme ortam�nda HTTPS gerekli de�ilse false olarak ayarlanabilir
});

// Oturum deste�i
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Oturum zaman a��m� s�resi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

// ProductService'i DI'a ekleme
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

var env = app.Environment;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Oturum middleware
app.UseSession();

// Kimlik do�rulama middleware
app.UseAuthentication();
app.UseAuthorization();

// Static dosyalar
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
