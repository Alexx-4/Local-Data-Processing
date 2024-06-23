using LocalDataProcessing.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDataProcessing.Core.Services
{
    public interface IPaymentDataProcessor
    {
        IAsyncEnumerable<IEnumerable<PaymentData>> ProcessPaymentDataAsync(string filePath);
    }
}
