using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CSV_Parse
{
    public class Transaction
    {
        [Index(2)]
        public string TransactionId { get; set; }
        [Index(14)]
        public string Type { get; set; } = null!;
        [Index(3)]
        public string CustomField { get; set; } = null!;
        [Index(12)]
        public decimal Rate { get; set; }
        [Index(11)]
        public decimal Amount { get; set; }
        [Index(10)]
        public string Currency { get; set; } = null!;
        [Index(4)]
        public DateOnly TradeDate { get; set; }
    }
}
