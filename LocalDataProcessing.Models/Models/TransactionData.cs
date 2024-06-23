using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDataProcessing.Core.Models
{
    public class TransactionData
    {
        public PaymentData PaymentData { get; set; }
        public int Count { get; set; }

        public TransactionData(int count, PaymentData paymentData)
        {
            Count = count;
            PaymentData = paymentData;
        }
    }

}
