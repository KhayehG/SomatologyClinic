using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace SomatologyClinic.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPayment(string method, decimal amount, string currency, string orderId, string token, int? installments = null);
    }


    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PaymentResult> ProcessPayment(string method, decimal amount, string currency, string orderId, string token, int? installments = null)
        {
            switch (method)
            {
                case "GooglePay":
                    return await ProcessGooglePay(amount, currency, orderId, token);
                case "ApplePay":
                    return await ProcessApplePay(amount, currency, orderId, token);
                case "PayFast":
                    return await ProcessPayFast(amount, currency, orderId, token);
                case "PayFlex":
                    return await ProcessPayFlex(amount, currency, orderId, token, installments.GetValueOrDefault());
                case "Cash":
                    return await ProcessCash(amount, currency, orderId);
                default:
                    throw new NotSupportedException($"Payment method {method} is not supported.");
            }
        }

        private async Task<PaymentResult> ProcessGooglePay(decimal amount, string currency, string orderId, string token)
        {
            var apiKey = _configuration["Payment:GooglePay:ApiKey"];
            var endpoint = _configuration["Payment:GooglePay:SandboxEndpoint"];

            var payload = new
            {
                amount,
                currency,
                orderId,
                token,
                apiKey
            };

            return await MakeApiCall(endpoint, payload);
        }

        private async Task<PaymentResult> ProcessApplePay(decimal amount, string currency, string orderId, string token)
        {
            var apiKey = _configuration["Payment:ApplePay:ApiKey"];
            var endpoint = _configuration["Payment:ApplePay:SandboxEndpoint"];

            var payload = new
            {
                amount,
                currency,
                orderId,
                token,
                apiKey
            };

            return await MakeApiCall(endpoint, payload);
        }

        private async Task<PaymentResult> ProcessPayFast(decimal amount, string currency, string orderId, string token)
        {
            var merchantId = _configuration["Payment:PayFast:MerchantId"];
            var merchantKey = _configuration["Payment:PayFast:MerchantKey"];
            var endpoint = _configuration["Payment:PayFast:SandboxEndpoint"];

            var payload = new
            {
                merchant_id = merchantId,
                merchant_key = merchantKey,
                amount,
                item_name = orderId,
                custom_str1 = token
            };

            return await MakeApiCall(endpoint, payload);
        }

        private async Task<PaymentResult> ProcessPayFlex(decimal amount, string currency, string orderId, string token, int installments)
        {
            var apiKey = _configuration["Payment:PayFlex:ApiKey"];
            var endpoint = _configuration["Payment:PayFlex:SandboxEndpoint"];

            var payload = new
            {
                amount,
                currency,
                orderId,
                token,
                apiKey,
                installments
            };

            return await MakeApiCall(endpoint, payload);
        }

        private async Task<PaymentResult> ProcessCash(decimal amount, string currency, string orderId)
        {
            // Mock Cash API for testing
            await Task.Delay(1000); // Simulate API call
            return new PaymentResult
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                Message = "Cash payment processed successfully"
            };
        }

        private async Task<PaymentResult> MakeApiCall(string endpoint, object payload)
        {
            var response = await _httpClient.PostAsync(endpoint, new StringContent(
                JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<PaymentResult>(content);
            }
            else
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = $"API call failed: {response.StatusCode} - {content}"
                };
            }
        }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
