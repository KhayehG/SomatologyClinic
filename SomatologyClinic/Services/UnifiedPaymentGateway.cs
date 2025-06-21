namespace SomatologyClinic.Services
{
    public class UnifiedPaymentGateway : IPaymentService
    {
        private readonly Random _random = new Random();

        public async Task<PaymentResult> ProcessPayment(string method, decimal amount, string currency, string orderId, string token, int? installments = null)
        {
            // Simulate network delay
            await Task.Delay(_random.Next(500, 2000));

            // Simulate a success rate of 90%
            bool isSuccessful = _random.NextDouble() < 0.9;

            if (isSuccessful)
            {
                return new PaymentResult
                {
                    Success = true,
                    TransactionId = GenerateTransactionId(),
                    Message = $"Payment of {amount} {currency} processed successfully via {method}."
                };
            }
            else
            {
                return new PaymentResult
                {
                    Success = false,
                    TransactionId = null,
                    Message = GetFailureMessage(method)
                };
            }
        }

        private string GenerateTransactionId()
        {
            return $"TRX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        private string GetFailureMessage(string method)
        {
            switch (method)
            {
                case "GooglePay":
                    return "Google Pay transaction failed. Please check your account balance and try again.";
                case "ApplePay":
                    return "Apple Pay transaction could not be completed. Please verify your card details.";
                case "PayFast":
                    return "PayFast payment unsuccessful. Please ensure you have sufficient funds.";
                case "PayFlex":
                    return "PayFlex transaction declined. Please contact PayFlex customer support for assistance.";
                case "Cash":
                    return "Cash payment recording failed. Please try again or contact support.";
                default:
                    return "Payment failed due to an unexpected error. Please try again later.";
            }
        }
    }

}
