# Desarrollo práctico requerido: SakilaApp, PayPhone y PayPal Sandbox

## 1. Objetivo y alcance

Este documento reúne la implementación y el procedimiento completo para comprobar SakilaApp: catálogo, carrito, orden, detalle, transacción, PayPhone API Link, PayPal Sandbox por redirección, botón PayPal embebido, actualización del estado de la orden, disminución de stock y comprobación de datos en PostgreSQL.

> **Seguridad:** nunca se deben escribir tokens, contraseñas, `ClientId` ni `ClientSecret` reales en este documento o en Git. Los valores entre `<...>` son marcadores que cada estudiante debe sustituir localmente mediante Secret Manager o variables de entorno.

## 2. Arquitectura implementada

El flujo real de la aplicación es:

```text
FilmStocks -> ShoppingCartItems -> PurchaseOrders
                                 -> PurchaseOrderDetails
                                 -> PaymentTransactions
                                            |
                         +------------------+------------------+
                         |                                     |
                  PayPhone API Link                    PayPal Sandbox
                                                      /              \
                                               redirección       botón embebido
                                                      \              /
                                                       captura aprobada
                                                              |
                                                orden/transacción = Paid
                                                FilmStocks.Stock -= cantidad
```

Los archivos que contienen **todo el código propio de este desarrollo** son:

```text
Program.cs
Controllers/StoreController.cs
Controllers/PaymentController.cs
Data/ApplicationDbContext.cs
Models/commerce/FilmStock.cs
Models/commerce/ShoopingCartItem.cs
Models/commerce/PurchaseOrder.cs
Models/commerce/PurchaseOrderDetail.cs
Models/commerce/PaymentTransaction.cs
Services/Payments/PayPhoneApiLinkService.cs
Services/Payments/PayPhoneLinkRequest.cs
Services/Payments/PayPalService.cs
Settings/PayPhoneSettings.cs
Settings/PayPalSettings.cs
Views/Store/Index.cshtml
Views/Store/Cart.cshtml
Views/Payment/Details.cshtml
Views/Payment/PayPalButton.cshtml
Data/MigrationsIdentity/20260710123917_ComercioPagosInicial.cs
Data/MigrationsIdentity/20260713153635_AgregarPaypalATransacciones.cs
```

No se incluyen como parte del desarrollo de pagos las librerías de `wwwroot/lib`, archivos compilados de `bin`, `obj` o `publish`, ni las páginas generadas automáticamente por ASP.NET Identity.

## 3. Requisitos

- .NET SDK 10.
- PostgreSQL disponible en el puerto configurado (actualmente `5433`).
- Base de datos Sakila y usuario con permisos de lectura/escritura.
- Cuenta de comercio PayPhone con token de API Link.
- Aplicación REST de PayPal en modo Sandbox y dos cuentas Sandbox: negocio y comprador.
- Aplicar las migraciones de Entity Framework Core.

Comprobación inicial:

```powershell
dotnet --version
dotnet restore
dotnet build
dotnet ef database update --context ApplicationDbContext
```

La cadena que actualmente usa el proyecto es:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=sakila;Username=cruduser;Password=crud123"
  }
}
```

En publicación, la contraseña debe salir de `appsettings.json` y configurarse como secreto:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=sakila;Username=<USUARIO>;Password=<CLAVE>"
```

## 4. Configuración de PayPhone

En el portal de PayPhone se obtiene el token de la aplicación/comercio. `PayPhoneSettings` contiene las claves de configuración y `Program.cs` enlaza esa sección y registra el cliente HTTP:

```csharp
builder.Services.Configure<PayPhoneSettings>(
    builder.Configuration.GetSection("PayPhone"));
builder.Services.AddHttpClient<PayPhoneApiLinkService>();
```

Configuración local segura:

```powershell
dotnet user-secrets init
dotnet user-secrets set "PayPhone:Token" "<TOKEN_PAYPHONE>"
dotnet user-secrets set "PayPhone:StoreId" "<STORE_ID>"
dotnet user-secrets list
```

Equivalente con variables de entorno (doble guion bajo representa `:`):

```powershell
$env:PayPhone__Token="<TOKEN_PAYPHONE>"
$env:PayPhone__StoreId="<STORE_ID>"
```

La solicitud implementada convierte dólares a centavos y crea el enlace:

```csharp
int amountInCents = (int)Math.Round(total * 100, MidpointRounding.AwayFromZero);

var request = new
{
    amount = amountInCents,
    amountWithoutTax = amountInCents,
    amountWithTax = 0,
    tax = 0,
    service = 0,
    tip = 0,
    currency = "USD",
    reference,
    clientTransactionId,
    additionalData = reference,
    oneTime = true,
    expireIn = 0,
    isAmountEditable = false
};

using var httpRequest = new HttpRequestMessage(
    HttpMethod.Post,
    "https://pay.payphonetodoesposible.com/api/Links");
httpRequest.Headers.Authorization =
    new AuthenticationHeaderValue("Bearer", _settings.Token);
httpRequest.Content = JsonContent.Create(request);
```

## 5. Configuración de PayPal Sandbox

Crear una aplicación REST Sandbox en PayPal Developer, copiar el Client ID y Secret de la aplicación y obtener las credenciales de la cuenta personal Sandbox que actuará como comprador. No se usa una cuenta PayPal real.

```powershell
dotnet user-secrets set "PayPal:ClientId" "<SANDBOX_CLIENT_ID>"
dotnet user-secrets set "PayPal:ClientSecret" "<SANDBOX_CLIENT_SECRET>"
dotnet user-secrets set "PayPal:BaseUrl" "https://api-m.sandbox.paypal.com"
dotnet user-secrets set "PayPal:CurrencyCode" "USD"
dotnet user-secrets set "PayPal:ReturnUrl" "http://localhost:5164/Payment/Success"
dotnet user-secrets set "PayPal:CancelUrl" "http://localhost:5164/Payment/Cancel"
```

Variables equivalentes para un servidor:

```text
PayPal__ClientId=<SANDBOX_CLIENT_ID>
PayPal__ClientSecret=<SANDBOX_CLIENT_SECRET>
PayPal__BaseUrl=https://api-m.sandbox.paypal.com
PayPal__CurrencyCode=USD
PayPal__ReturnUrl=https://app.ejemplo.com/Payment/Success
PayPal__CancelUrl=https://app.ejemplo.com/Payment/Cancel
```

Registro en `Program.cs`:

```csharp
builder.Services.Configure<PayPalSettings>(
    builder.Configuration.GetSection("PayPal"));
builder.Services.AddHttpClient<PayPalService>();
```

El servicio solicita un token OAuth 2.0 con `ClientId:ClientSecret`, crea una orden mediante `POST /v2/checkout/orders` y la captura con `POST /v2/checkout/orders/{id}/capture`. Una captura solo se considera exitosa cuando PayPal devuelve `COMPLETED`.

## 6. Código del dominio y persistencia

Las relaciones se registran en `ApplicationDbContext`:

```csharp
public DbSet<FilmStock> FilmStocks => Set<FilmStock>();
public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();
public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
```

Campos principales:

```csharp
public class PurchaseOrder
{
    public int PurchaseOrderId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pending";
    public ICollection<PurchaseOrderDetail> Details { get; set; } = new List<PurchaseOrderDetail>();
}

public class PurchaseOrderDetail
{
    public int PurchaseOrderDetailId { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public int FilmStockId { get; set; }
    public string FilmTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class PaymentTransaction
{
    public int PaymentTransactionId { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public string Provider { get; set; } = "PayPhone";
    public string ClientTransactionId { get; set; } = string.Empty;
    public string? PayphonePaymentUrl { get; set; }
    public string? PayphoneTransactionId { get; set; }
    public string? PayPalOrderId { get; set; }
    public string? PayPalCaptureId { get; set; }
    public string? PayPalApprovalUrl { get; set; }
    public string? GatewayResponse { get; set; }
    public int AmountInCents { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}
```

## 7. Flujo carrito, orden y detalle

1. `Store/Index` inicializa hasta 20 películas con stock 10 si el catálogo comercial está vacío.
2. `AddToCart` valida cantidad y disponibilidad y crea/actualiza `ShoppingCartItem` para el correo autenticado.
3. `Cart` carga los productos y calcula cada subtotal.
4. `Checkout` vuelve a validar stock, crea `PurchaseOrder` en estado `Pending`, agrega todos los `PurchaseOrderDetail`, calcula el total y elimina los ítems del carrito.
5. Según `provider`, redirige a PayPhone, PayPal o a la pantalla del botón embebido.

Código decisivo de `Checkout`:

```csharp
var order = new PurchaseOrder { UserEmail = userEmail, Status = "Pending" };

foreach (var item in cartItems)
{
    var subtotal = item.Quantity * item.FilmStock.UnitPrice;
    order.Details.Add(new PurchaseOrderDetail
    {
        FilmStockId = item.FilmStockId,
        FilmTitle = item.FilmStock.Title,
        Quantity = item.Quantity,
        UnitPrice = item.FilmStock.UnitPrice,
        Subtotal = subtotal
    });
    order.Total += subtotal;
}

_context.PurchaseOrders.Add(order);
_context.ShoppingCartItems.RemoveRange(cartItems);
await _context.SaveChangesAsync();
```

## 8. Prueba de PayPhone mediante API Link

1. Ejecutar `dotnet run --launch-profile http` y abrir `http://localhost:5164`.
2. Iniciar sesión, entrar en **Tienda**, agregar una película y abrir el carrito.
3. Pulsar **Generar orden y pagar con PayPhone**.
4. Confirmar que se creó una orden `Pending` y una transacción PayPhone `Pending` con `ClientTransactionId`, monto en centavos y `PayphonePaymentUrl`.
5. Pulsar **Abrir link de pago PayPhone** y completar la prueba en el ambiente autorizado.
6. Guardar evidencia del enlace, respuesta y estado devuelto por la pasarela.

La implementación actual genera y conserva el API Link. Para una confirmación automática de PayPhone en producción debe agregarse el endpoint oficial de confirmación/webhook, validar criptográficamente la notificación y solo entonces ejecutar la misma operación de marcado como pagado. El botón administrativo **Marcar como pagado para demostración** prueba la lógica interna, pero no sustituye una confirmación real de la pasarela.

## 9. Prueba de PayPal Sandbox por redirección

1. Seleccionar **Pagar con PayPal Sandbox**.
2. La aplicación crea la orden PayPal y muestra **Abrir PayPal Sandbox**.
3. Abrirla e iniciar sesión con la cuenta personal Sandbox de prueba.
4. Aprobar el pago. PayPal retorna a `/Payment/Success?token=<PAYPAL_ORDER_ID>`.
5. `Success` captura la orden. Si el estado es `COMPLETED`, cambia la transacción y la orden a `Paid` y descuenta el stock.
6. Para probar cancelación, cancelar en PayPal: el retorno a `/Payment/Cancel` deja la transacción `Cancelled` y no descuenta stock.

## 10. Prueba del botón PayPal embebido

El botón se renderiza dentro de `Views/Payment/PayPalButton.cshtml` mediante el SDK oficial:

```html
<div id="paypal-button-container" style="max-width: 480px;"></div>
<script src="https://www.paypal.com/sdk/js?client-id=@paypalClientId&currency=@paypalCurrency&intent=capture&components=buttons"></script>
```

El JavaScript llama al servidor para que el secreto nunca llegue al navegador:

```javascript
paypal.Buttons({
    createOrder: async () => {
        const response = await fetch("@createOrderUrl", {
            method: "POST",
            headers: { "Accept": "application/json" }
        });
        const payload = await response.json();
        if (!response.ok || !payload.success)
            throw new Error(payload.message || "No se pudo crear la orden PayPal.");
        return payload.orderId;
    },
    onApprove: async (data) => {
        const response = await fetch(
            "@captureOrderUrl?paypalOrderId=" + encodeURIComponent(data.orderID),
            { method: "POST", headers: { "Accept": "application/json" } });
        const payload = await response.json();
        if (!response.ok || !payload.success)
            throw new Error(payload.message || "No se pudo capturar el pago PayPal.");
        window.location.href = payload.redirectUrl;
    }
}).render("#paypal-button-container");
```

Validación visual y funcional:

- El botón debe aparecer **dentro** de la página de SakilaApp, no como un enlace aislado.
- Las herramientas de desarrollador no deben mostrar errores del SDK ni un `ClientSecret`.
- Al pulsarlo debe abrir el flujo Sandbox y permitir usar la cuenta compradora.
- Al aprobar debe volver al detalle, mostrar `Paid`, PayPal Order ID y Capture ID.
- Al cancelar no debe descontar existencias.

## 11. Estado y disminución de stock

Este es el único método común que confirma la operación:

```csharp
private async Task MarkPaymentAsPaidAsync(PaymentTransaction payment)
{
    if (payment.Status == "Paid") return; // evita descontar dos veces

    payment.Status = "Paid";
    payment.ConfirmedAt = DateTime.UtcNow;
    payment.PurchaseOrder.Status = "Paid";

    foreach (var detail in payment.PurchaseOrder.Details)
    {
        var stock = await _context.FilmStocks.FindAsync(detail.FilmStockId);
        if (stock != null)
            stock.Stock -= detail.Quantity;
    }
}
```

Matriz esperada:

| Resultado | Transacción | Orden | Stock |
|---|---|---|---|
| Creación del pago | `Pending` | `Pending` | Sin cambio |
| PayPal `COMPLETED` | `Paid` | `Paid` | Disminuye según el detalle |
| Cancelación PayPal | `Cancelled` | `Pending` | Sin cambio |
| Error al crear PayPal | `Failed` | `Pending` | Sin cambio |
| Confirmación administrativa de demostración | `Paid` | `Paid` | Disminuye una sola vez |

## 12. Verificación en PostgreSQL

Entrar con `psql` (la opción `-W` solicita la contraseña sin escribirla en el historial):

```powershell
psql -h localhost -p 5433 -U cruduser -d sakila -W
```

Consultar las tablas creadas por EF Core. PostgreSQL distingue mayúsculas porque los nombres están entre comillas:

```sql
SELECT "PurchaseOrderId", "UserEmail", "CreatedAt", "Total", "Status"
FROM "PurchaseOrders"
ORDER BY "PurchaseOrderId" DESC;

SELECT "PurchaseOrderDetailId", "PurchaseOrderId", "FilmStockId",
       "FilmTitle", "Quantity", "UnitPrice", "Subtotal"
FROM "PurchaseOrderDetails"
ORDER BY "PurchaseOrderDetailId" DESC;

SELECT "PaymentTransactionId", "PurchaseOrderId", "Provider",
       "ClientTransactionId", "AmountInCents", "Status", "ConfirmedAt",
       "PayphonePaymentUrl", "PayPalOrderId", "PayPalCaptureId"
FROM "PaymentTransactions"
ORDER BY "PaymentTransactionId" DESC;

SELECT "FilmStockId", "Title", "Stock", "UnitPrice"
FROM "FilmStocks"
ORDER BY "FilmStockId";
```

Consulta integral para una evidencia:

```sql
SELECT o."PurchaseOrderId", o."Status" AS "OrderStatus", o."Total",
       d."FilmTitle", d."Quantity", d."Subtotal",
       t."Provider", t."Status" AS "PaymentStatus",
       t."PayPalOrderId", t."PayPalCaptureId", t."ConfirmedAt",
       s."Stock" AS "CurrentStock"
FROM "PurchaseOrders" o
JOIN "PurchaseOrderDetails" d ON d."PurchaseOrderId" = o."PurchaseOrderId"
JOIN "PaymentTransactions" t ON t."PurchaseOrderId" = o."PurchaseOrderId"
JOIN "FilmStocks" s ON s."FilmStockId" = d."FilmStockId"
WHERE o."PurchaseOrderId" = <ID_ORDEN>;
```

Antes y después del pago se debe registrar el valor de `FilmStocks.Stock`. La diferencia esperada es exactamente `PurchaseOrderDetails.Quantity`.

## 13. Lista de evidencias

- Compilación exitosa y aplicación iniciada en el puerto configurado.
- Catálogo, artículo agregado y carrito con total correcto.
- Orden y detalles `Pending` después del checkout.
- API Link de PayPhone generado y abierto.
- Inicio de sesión y aprobación con comprador PayPal Sandbox.
- Botón PayPal visible dentro de SakilaApp.
- Pantalla final con transacción `Paid` y los identificadores de PayPal.
- Consulta PostgreSQL de orden, detalles y transacción.
- Stock antes y después; diferencia igual a la cantidad comprada.
- Caso cancelado o fallido sin disminución de stock.

## 14. Relación con la publicación de aplicaciones web

Cuando SakilaApp deja de ejecutarse solo en el equipo del desarrollador, su dirección, puerto, secretos y servicios externos cambian. Las **rutas de retorno** indican a PayPal dónde devolver al usuario después de aprobar o cancelar; deben usar el dominio público y HTTPS del despliegue. Una ruta `localhost` solo funciona en la máquina local y no es accesible desde Internet.

Las **variables de entorno y almacenes de secretos** separan configuración y código. Permiten usar credenciales Sandbox en desarrollo y credenciales de producción en el servidor sin modificar ni volver a publicar el código. También impiden que claves de base de datos y pasarelas terminen en Git, capturas o entregables.

Los **puertos** determinan dónde escucha ASP.NET Core y cómo un proxy inverso (IIS, Nginx, Apache, contenedor o plataforma cloud) reenvía las solicitudes. En producción, el usuario normalmente accede por 443/HTTPS aunque internamente Kestrel escuche otro puerto. El firewall, el proxy y `ASPNETCORE_URLS` deben coincidir.

Los **ambientes de prueba** protegen dinero y datos reales. Sandbox reproduce el protocolo de la pasarela con cuentas y fondos ficticios; producción usa otros endpoints y credenciales. Mezclar Client ID, Secret o URL entre ambientes provoca errores y puede ejecutar operaciones reales accidentalmente.

Finalmente, una aplicación publicada requiere HTTPS, logs sin secretos, migraciones controladas, respaldos, manejo de errores, confirmación servidor-a-servidor/webhooks e idempotencia. No se debe confiar únicamente en que el navegador regrese a `Success`: el usuario puede cerrar la pestaña o un tercero puede intentar invocar la ruta. El servidor debe verificar el resultado con la pasarela antes de marcar `Paid` y descontar stock.

## 15. Comandos finales de ejecución

```powershell
dotnet restore
dotnet ef database update --context ApplicationDbContext
dotnet build
dotnet run --launch-profile http
```

URL local definida en `Properties/launchSettings.json`:

```text
http://localhost:5164
```

Con esto quedan documentadas la configuración, implementación, ejecución, pruebas, persistencia y consideraciones de despliegue del desarrollo práctico.
