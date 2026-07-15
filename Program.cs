using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using SakilaApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SakilaApp.Services;
using SakilaApp.Settings;
using SakilaApp.Services.Payments;


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
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 6;
    })
    // AddDefaultIdentity registra los servicios de Identity y sus proveedores de tokens.
    // El proveedor "AuthenticatorTokenProvider" es el que valida los codigos TOTP
    // de apps como Microsoft Authenticator o Google Authenticator; la app no llama
    // a esos proveedores externos para validar el codigo.
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender, GmailEmailSender>();

builder.Services.Configure<PayPhoneSettings>(
    builder.Configuration.GetSection("PayPhone"));

builder.Services.AddHttpClient<PayPhoneApiLinkService>();

builder.Services.Configure<PayPalSettings>(
    builder.Configuration.GetSection("PayPal"));

builder.Services.AddHttpClient<PayPalService>();

// La aplicacion no se ejecutaba porque AddDefaultIdentity estaba registrado dos veces.
// Eso intentaba crear dos veces el esquema de autenticacion "Identity.Application" y ASP.NET Core
// detenia el arranque con: "Scheme already exists: Identity.Application".

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

// Registrar Google solo cuando hay credenciales configuradas. Si se registra con
// ClientId/ClientSecret vacios, OAuth falla al validar opciones y bloquea el login.
if (!string.IsNullOrWhiteSpace(googleClientId) &&
    !string.IsNullOrWhiteSpace(googleClientSecret))
{
    builder.Services
        .AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        });
}

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

// Si la aplicacion vuelve a fallar con "address already in use", no es error de compilacion:
// significa que el puerto obligatorio localhost:5164 ya esta ocupado por otro proceso.
// Para ejecutar en el puerto requerido, primero hay que cerrar el proceso que use el puerto 5164.
app.Run();
