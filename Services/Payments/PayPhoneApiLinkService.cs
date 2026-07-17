using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SakilaApp.Settings;

namespace SakilaApp.Services.Payments;

public class PayPhoneApiLinkService
{
    private readonly HttpClient _httpClient;
    private readonly PayPhoneSettings _settings;

    public PayPhoneApiLinkService(
        HttpClient httpClient,
        IOptions<PayPhoneSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
    }

    public async Task<string> CreatePaymentLinkAsync(
        decimal total,
        string clientTransactionId,
        string reference)
    {
        EnsureConfigured();

        int amountInCents = (int)Math.Round(
            total * 100,
            MidpointRounding.AwayFromZero);

        var request = new
        {
            amount = amountInCents,
            amountWithoutTax = amountInCents,
            currency = "USD",
            clientTransactionId,
            storeId = _settings.StoreId,
            reference
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "https://pay.payphonetodoesposible.com/api/Links");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.Token);

        httpRequest.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(httpRequest);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"PayPhone respondió con error: {content}");
        }

        return content.Trim('"');
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_settings.Token))
        {
            throw new InvalidOperationException(
                "Configura PayPhone:Token mediante User Secrets o la variable PayPhone__Token.");
        }

        if (string.IsNullOrWhiteSpace(_settings.StoreId))
        {
            throw new InvalidOperationException(
                "Configura PayPhone:StoreId mediante User Secrets o la variable PayPhone__StoreId.");
        }
    }
}
