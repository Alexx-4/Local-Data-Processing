using LocalDataProcessing.Core.Models;
using LocalDataProcessing.Core.Services;
using System.Diagnostics;


namespace LocalDataProcessing.App
{

    public class PaymentCsvWorker : BackgroundService
    {
        private readonly IPaymentDataProcessor _paymentDataProcessor;
        public static Dictionary<string, TransactionData> _transactionsByCountry;

        public PaymentCsvWorker(IPaymentDataProcessor paymentDataProcessor)
        {
            _paymentDataProcessor = paymentDataProcessor;
            _transactionsByCountry = new Dictionary<string, TransactionData>();
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string filePath = "paymentData.csv";
            var processedLines = 0;

            try
            {
                await foreach (var item in _paymentDataProcessor.ProcessPaymentDataAsync(filePath).WithCancellation(token))
                {
                    await Task.WhenAll(item.Select(Process));

                    processedLines += item.Count();
                    Console.Clear(); Console.Write("Processing... ");
                }

                var totalTransactions = 0;

                foreach (var item in _transactionsByCountry.Values)
                {
                    totalTransactions += item.Count;

                    Console.WriteLine($"\n ==== Country: {item.PaymentData.IssuerCountry} ==== ");
                    Console.WriteLine($" Transactions: {item.Count} ");
                    Console.WriteLine(@$" Example: {item.PaymentData}");
                    Console.WriteLine($" =================================================== ");
                }

                Console.WriteLine($"\nTOTAL TRANSACTIONS: {totalTransactions}\n");

                stopwatch.Stop();
                Console.WriteLine($"Processed {processedLines} lines in " + stopwatch.Elapsed.TotalSeconds + " s.");
            }


            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        private async Task Process(PaymentData item)
        {
            if (IsBeneluxCountry(item.IssuerCountry) && item.PaymentMethod.ToUpper() == "MASTERCARD" && item.ResponseDescription.ToUpper() == "APPROVED")
            {
                try
                {
                    var transaction = _transactionsByCountry[item.IssuerCountry];
                    transaction.Count++;
                }
                catch (KeyNotFoundException ex)
                {
                    _transactionsByCountry[item.IssuerCountry] = new TransactionData(count: 1, paymentData: item);
                }
            }
        }

        private bool IsBeneluxCountry(string country) => country == "BE" || country == "NL" || country == "LU";

    }
}
