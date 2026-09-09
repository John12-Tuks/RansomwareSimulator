using RansomwareSim.model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RansomwareSim.Service
{
    internal class PaymentService
    {
        public async Task<PaymentResponse> InitializePayment(string email, int amount)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    "sk_test_7fa4b0f5e087b85db0a0b84ad8f2b7b835033e35");

            var request = new
            {
                email = email,
                amount = amount
            };

            var json = JsonSerializer.Serialize(request);

            var response = await client.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseJson);

            var data = doc.RootElement.GetProperty("data");

            return new PaymentResponse
            {
                AuthorizationUrl = data.GetProperty("authorization_url").GetString(),
                Reference = data.GetProperty("reference").GetString()
            };
        }

        public async Task<string> VerifyPayment(string reference)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    "sk_test_7fa4b0f5e087b85db0a0b84ad8f2b7b835033e35");

            var response = await client.GetAsync(
                $"https://api.paystack.co/transaction/verify/{reference}");

            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return "error";
            }

            using JsonDocument doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out JsonElement data))
            {
                return "error";
            }

            string status = data.GetProperty("status").GetString() ?? "";

            return status.Trim().ToLower();
        }
    }
}