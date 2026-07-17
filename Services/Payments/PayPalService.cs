using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SakilaApp.Settings;

namespace SakilaApp.Services.Payments;

public class PayPalOrderResult
{
    public string OrderId { get; set; } = string.Empty;
    public string ApprovalUrl { get; set; } = string.Empty;
    public string RawResponse { get; set; } = string.Empty;
}

public class PayPalCaptureResult
{
    public string Status { get; set; } = string.Empty;
    public string CaptureId { get; set; } = string.Empty;
    public string RawResponse { get; set; } = string.Empty;
}

public class PayPalService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly PayPalSettings _settings;

    public PayPalService(HttpClient httpClient, IOptions<PayPalSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
    }

    public async Task<PayPalOrderResult> CreateOrderAsync(
        decimal total,
        string reference,
        bool includeRedirectUrls = true)
    {
        var accessToken = await GetAccessTokenAsync();
        var amountValue = total.ToString("0.00", CultureInfo.InvariantCulture);

        var purchaseUnit = new
        {
            reference_id = reference,
            description = reference,
            custom_id = reference,
            amount = new
            {
                currency_code = _settings.CurrencyCode,
                value = amountValue
            }
        };

        object requestBody;

        if (includeRedirectUrls &&
            (string.IsNullOrWhiteSpace(_settings.ReturnUrl) ||
             string.IsNullOrWhiteSpace(_settings.CancelUrl)))
        {
            throw new InvalidOperationException(
                "Configura PayPal:ReturnUrl y PayPal:CancelUrl para el flujo de redireccion.");
        }

        if (includeRedirectUrls)
        {
            requestBody = new
            {
                intent = "CAPTURE",
                purchase_units = new[] { purchaseUnit },
                application_context = new
                {
                    brand_name = "Sakila App",
                    return_url = _settings.ReturnUrl,
                    cancel_url = _settings.CancelUrl,
                    user_action = "PAY_NOW"
                }
            };
        }
        else
        {
            requestBody = new
            {
                intent = "CAPTURE",
                purchase_units = new[] { purchaseUnit }
            };
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildApiUrl("/v2/checkout/orders"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("Prefer", "return=representation");
        request.Content = JsonContent.Create(requestBody, options: JsonOptions);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"PayPal respondio con error al crear la orden: {content}");
        }

        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;

        return new PayPalOrderResult
        {
            OrderId = root.GetProperty("id").GetString() ?? string.Empty,
            ApprovalUrl = FindApprovalUrl(root),
            RawResponse = content
        };
    }

    public async Task<PayPalCaptureResult> CaptureOrderAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("El id de orden PayPal es obligatorio.", nameof(orderId));
        }

        var accessToken = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildApiUrl($"/v2/checkout/orders/{Uri.EscapeDataString(orderId)}/capture"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("Prefer", "return=representation");
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"PayPal respondio con error al capturar la orden: {content}");
        }

        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;

        return new PayPalCaptureResult
        {
            Status = root.TryGetProperty("status", out var status)
                ? status.GetString() ?? string.Empty
                : string.Empty,
            CaptureId = FindCaptureId(root),
            RawResponse = content
        };
    }

    private async Task<string> GetAccessTokenAsync()
    {
        EnsureConfigured();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildApiUrl("/v1/oauth2/token"));

        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"PayPal respondio con error al solicitar token: {content}");
        }

        using var document = JsonDocument.Parse(content);
        return document.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_settings.ClientId) ||
            string.IsNullOrWhiteSpace(_settings.ClientSecret))
        {
            throw new InvalidOperationException(
                "Configura PayPal:ClientId y PayPal:ClientSecret mediante User Secrets o las variables PayPal__ClientId y PayPal__ClientSecret.");
        }

        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
        {
            throw new InvalidOperationException("Configura PayPal:BaseUrl.");
        }
    }

    private string BuildApiUrl(string path)
    {
        return $"{_settings.BaseUrl.TrimEnd('/')}{path}";
    }

    private static string FindApprovalUrl(JsonElement root)
    {
        if (!root.TryGetProperty("links", out var links) ||
            links.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        foreach (var link in links.EnumerateArray())
        {
            if (link.TryGetProperty("rel", out var rel) &&
                string.Equals(rel.GetString(), "approve", StringComparison.OrdinalIgnoreCase) &&
                link.TryGetProperty("href", out var href))
            {
                return href.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    private static string FindCaptureId(JsonElement root)
    {
        if (!root.TryGetProperty("purchase_units", out var purchaseUnits) ||
            purchaseUnits.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        foreach (var purchaseUnit in purchaseUnits.EnumerateArray())
        {
            if (!purchaseUnit.TryGetProperty("payments", out var payments) ||
                !payments.TryGetProperty("captures", out var captures) ||
                captures.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var capture in captures.EnumerateArray())
            {
                if (capture.TryGetProperty("id", out var id))
                {
                    return id.GetString() ?? string.Empty;
                }
            }
        }

        return string.Empty;
    }
}
