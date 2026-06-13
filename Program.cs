using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using SakilaApp.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();



builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")));

builder.Services.AddDbContext<SakilaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); //aqui podemos definir en que lenguaje vamos a trabajar

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// CORREGIDO: Identity necesita leer la cookie de autenticacion antes de aplicar autorizacion.
// Sin UseAuthentication(), la app no reconoce al usuario aunque haya iniciado sesion.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    // ANTES: pattern: "{controller=Home}/{action=Index}/{id?}");
    // CORREGIDO: ahora el index principal abre directamente Films/Index.
    pattern: "{controller=Films}/{action=Index}/{id?}");

// CORREGIDO: las pantallas de Login/Register de Identity son Razor Pages.
// Sin MapRazorPages(), rutas como /Identity/Account/Login no quedan registradas.
app.MapRazorPages();

//nuevo archivo llamado seeder y lo llamamos
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}
app.Run();
