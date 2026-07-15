using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Data;
using SakilaApp.Models.Commerce;
using SakilaApp.Services.Payments;

namespace SakilaApp.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PayPhoneApiLinkService _payPhoneService;
    private readonly PayPalService _payPalService;

    public PaymentController(
        ApplicationDbContext context,
        PayPhoneApiLinkService payPhoneService,
        PayPalService payPalService)
    {
        _context = context;
        _payPhoneService = payPhoneService;
        _payPalService = payPalService;
    }

    public async Task<IActionResult> CreateLink(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null) return NotFound();

        string clientTransactionId = GenerateClientTransactionId();
        string reference = $"Orden Sakila #{order.PurchaseOrderId}";

        string link = await _payPhoneService.CreatePaymentLinkAsync(
            order.Total,
            clientTransactionId,
            reference);

        var payment = new PaymentTransaction
        {
            PurchaseOrderId = order.PurchaseOrderId,
            Provider = "PayPhone",
            ClientTransactionId = clientTransactionId,
            PayphonePaymentUrl = link,
            AmountInCents = ToCents(order.Total),
            Status = "Pending"
        };

        _context.PaymentTransactions.Add(payment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
    }

    public async Task<IActionResult> CreatePayPalOrder(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null) return NotFound();

        string clientTransactionId = GenerateClientTransactionId();
        string reference = $"Orden Sakila #{order.PurchaseOrderId}";

        PayPalOrderResult result;

        try
        {
            result = await _payPalService.CreateOrderAsync(
                order.Total,
                reference,
                includeRedirectUrls: true);
        }
        catch (InvalidOperationException ex)
        {
            var failedPayment = new PaymentTransaction
            {
                PurchaseOrderId = order.PurchaseOrderId,
                Provider = "PayPal",
                ClientTransactionId = clientTransactionId,
                GatewayResponse = ex.Message,
                AmountInCents = ToCents(order.Total),
                Status = "Failed"
            };

            _context.PaymentTransactions.Add(failedPayment);
            await _context.SaveChangesAsync();
            TempData["Error"] = ex.Message;

            return RedirectToAction(nameof(Details), new { id = failedPayment.PaymentTransactionId });
        }

        if (string.IsNullOrWhiteSpace(result.ApprovalUrl))
        {
            throw new InvalidOperationException("PayPal no devolvio una URL de aprobacion.");
        }

        var payment = new PaymentTransaction
        {
            PurchaseOrderId = order.PurchaseOrderId,
            Provider = "PayPal",
            ClientTransactionId = clientTransactionId,
            PayPalOrderId = result.OrderId,
            PayPalApprovalUrl = result.ApprovalUrl,
            GatewayResponse = result.RawResponse,
            AmountInCents = ToCents(order.Total),
            Status = "Pending"
        };

        _context.PaymentTransactions.Add(payment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
    }

    public async Task<IActionResult> PayPalButton(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null) return NotFound();

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayPalButtonOrderJson(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.PurchaseOrderId == orderId);

        if (order == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Json(new
            {
                success = false,
                message = "Orden no encontrada."
            });
        }

        if (order.Status == "Paid")
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Json(new
            {
                success = false,
                message = "La orden ya esta pagada."
            });
        }

        try
        {
            string clientTransactionId = GenerateClientTransactionId();
            string reference = $"Orden Sakila #{order.PurchaseOrderId}";

            var result = await _payPalService.CreateOrderAsync(
                order.Total,
                reference,
                includeRedirectUrls: false);

            var payment = new PaymentTransaction
            {
                PurchaseOrderId = order.PurchaseOrderId,
                Provider = "PayPal",
                ClientTransactionId = clientTransactionId,
                PayPalOrderId = result.OrderId,
                PayPalApprovalUrl = result.ApprovalUrl,
                GatewayResponse = result.RawResponse,
                AmountInCents = ToCents(order.Total),
                Status = "Pending"
            };

            _context.PaymentTransactions.Add(payment);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                orderId = result.OrderId
            });
        }
        catch (InvalidOperationException ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CapturePayPalButtonOrderJson(string paypalOrderId)
    {
        if (string.IsNullOrWhiteSpace(paypalOrderId))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Json(new
            {
                success = false,
                message = "No llego el id de la orden PayPal."
            });
        }

        var payment = await FindPayPalPaymentAsync(paypalOrderId);

        if (payment == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Json(new
            {
                success = false,
                message = "Transaccion PayPal no encontrada."
            });
        }

        try
        {
            if (payment.Status != "Paid")
            {
                var capture = await _payPalService.CaptureOrderAsync(paypalOrderId);
                payment.PayPalCaptureId = capture.CaptureId;
                payment.GatewayResponse = capture.RawResponse;

                if (capture.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    await MarkPaymentAsPaidAsync(payment);
                }
                else
                {
                    payment.Status = capture.Status;
                }

                await _context.SaveChangesAsync();
            }

            return Json(new
            {
                success = payment.Status == "Paid",
                status = payment.Status,
                redirectUrl = Url.Action(nameof(Details), new { id = payment.PaymentTransactionId })
            });
        }
        catch (InvalidOperationException ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    public async Task<IActionResult> Success(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Index", "Store");
        }

        var payment = await FindPayPalPaymentAsync(token);
        if (payment == null) return NotFound();

        if (payment.Status != "Paid")
        {
            var capture = await _payPalService.CaptureOrderAsync(token);
            payment.PayPalCaptureId = capture.CaptureId;
            payment.GatewayResponse = capture.RawResponse;

            if (capture.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                await MarkPaymentAsPaidAsync(payment);
            }
            else
            {
                payment.Status = capture.Status;
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
    }

    public async Task<IActionResult> Cancel(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Index", "Store");
        }

        var payment = await _context.PaymentTransactions
            .FirstOrDefaultAsync(p => p.PayPalOrderId == token);

        if (payment == null) return NotFound();

        if (payment.Status == "Pending")
        {
            payment.Status = "Cancelled";
            payment.GatewayResponse = "Pago cancelado por el usuario en PayPal.";
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = payment.PaymentTransactionId });
    }

    public async Task<IActionResult> Details(int id)
    {
        var payment = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.PaymentTransactionId == id);

        if (payment == null) return NotFound();
        return View(payment);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var payment = await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.PaymentTransactionId == id);

        if (payment == null) return NotFound();

        await MarkPaymentAsPaidAsync(payment);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<PaymentTransaction?> FindPayPalPaymentAsync(string paypalOrderId)
    {
        return await _context.PaymentTransactions
            .Include(p => p.PurchaseOrder)
            .ThenInclude(o => o.Details)
            .FirstOrDefaultAsync(p => p.PayPalOrderId == paypalOrderId);
    }

    private async Task MarkPaymentAsPaidAsync(PaymentTransaction payment)
    {
        if (payment.Status == "Paid")
        {
            return;
        }

        payment.Status = "Paid";
        payment.ConfirmedAt = DateTime.UtcNow;
        payment.PurchaseOrder.Status = "Paid";

        foreach (var detail in payment.PurchaseOrder.Details)
        {
            var stock = await _context.FilmStocks.FindAsync(detail.FilmStockId);
            if (stock != null)
            {
                stock.Stock -= detail.Quantity;
            }
        }
    }

    private static string GenerateClientTransactionId()
    {
        return DateTime.UtcNow.ToString("yyMMddHHmmssfff", CultureInfo.InvariantCulture);
    }

    private static int ToCents(decimal amount)
    {
        return (int)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
    }
}
