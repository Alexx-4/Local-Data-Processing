using CsvHelper;
using CsvHelper.Configuration;
using LocalDataProcessing.Core.Models;
using LocalDataProcessing.Core.Services;
using System.Globalization;



namespace LocalDataProcessing.Logic.Services
{
    public class PaymentDataProcessor: IPaymentDataProcessor
    {
        public PaymentDataProcessor() { }

        public async IAsyncEnumerable<IEnumerable<PaymentData>> ProcessPaymentDataAsync(string filePath)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", string.Empty)
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, configuration))
            {
                await csv.ReadAsync();
                csv.ReadHeader();

                var batch = new List<PaymentData>();
                var batchSize = 100;

                while (await csv.ReadAsync())
                {
                    var paymentData = csv.GetRecord<PaymentData>();
                    batch.Add(paymentData);

                    if(batch.Count > batchSize)
                    {
                        yield return batch;
                        batch = new List<PaymentData>();
                    }
                }

                if (batch.Count > 0) yield return batch;
                
            }
        }
    }
}
