using BenchmarkDotNet.Attributes;
using nietras.SeparatedValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CSV_Parse
{
    [MemoryDiagnoser]
    [ShortRunJob]
    public class Benchmark
    {
        public string filePath = @"..\..\..\..\..\..\..\..\Cashflow Settlement Report for Sep - 1k lines.csv";

        [Benchmark]
        public void SepParseWithLoggingV3()
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

            //var result = new List<StandardExcelRow>();

            decimal batchTotal = 0;

            var batchIndex = 1;

            foreach (var readRow in reader)
            {
                var row = MapRow(readRow);
                //result.Add(row);

                if (!row.DetailNo.HasValue)
                {
                    var total = row.Amount;
                    if (batchTotal == total)
                    {
                        Console.WriteLine();
                        Console.Write($"TOTAL: {total} ");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write($"TOTAL amount is different than the calculated amount: {total} <> {batchTotal}");
                        Console.WriteLine();
                    }

                    batchTotal = 0;
                    total = 0;

                    continue;
                }
                else if (row.DetailNo == 1)
                {
                    Console.WriteLine();
                    Console.Write($"BATCH  {batchIndex}:");
                    Console.WriteLine();
                    batchIndex++;
                }

                batchTotal += row.Amount;
                //ConsoleWriteLine(row);
            }
        }

        [Benchmark]
        public void SepParseWithLoggingV2()
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

            decimal batchTotal = 0;

            var batchIndex = 1;

            foreach (var readRow in reader)
            {
                if (readRow[1].Span.IsEmpty)
                {
                    var total = readRow[11].Parse<decimal>();
                    if (batchTotal == total)
                    {
                        Console.WriteLine();
                        Console.Write($"TOTAL: {total} ");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write($"TOTAL amount is different than the calculated amount: {total} <> {batchTotal}");
                        Console.WriteLine();
                    }

                    batchTotal = 0;
                    total = 0;

                    continue;
                }
                else if (readRow[1].TryParse(out int detailNo) && detailNo == 1)
                {
                    Console.WriteLine();
                    Console.Write($"BATCH  {batchIndex}:");
                    Console.WriteLine();
                    batchIndex++;
                }

                var detail = MapRow(readRow);
                //ConsoleWriteLine(detail);

                batchTotal += detail.Amount;

                //Console.WriteLine();
            }
        }

        [Benchmark]
        public void SepParseWithLoggingV1()
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

            //var batch = new List<StandardExcelRow>();
            decimal batchTotal = 0;

            var batchIndex = 1;

            foreach (var readRow in reader)
            {
                if (string.IsNullOrEmpty(readRow[1].ToString()))
                {
                    var t = readRow[11].ToString();
                    if (decimal.TryParse(t, out decimal total) && batchTotal == total)
                    {
                        Console.WriteLine();
                        Console.Write($"TOTAL: {t} ");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write($"TOTAL is not the same: {total} - {batchTotal}");
                        Console.WriteLine();
                    }

                    batchTotal = 0;
                    total = 0;

                    continue;
                }
                else if (readRow[1].ToString() == "1")
                {
                    Console.WriteLine();
                    Console.Write($"BATCH  {batchIndex}:");
                    Console.WriteLine();
                    batchIndex++;
                }

                var detail = MapRow(readRow);
                //ConsoleWriteLine(detail);

                batchTotal += detail.Amount;

                //Console.WriteLine();
            }
        }

        private static StandardExcelRow MapRow(SepReader.Row row)
        {
            return new StandardExcelRow
            {
                RowNo = row["Row No"].Parse<int>(),
                DetailNo = row["Detail No"].TryParse<int>(),
                SchemeTransactionID = row["Scheme Transaction ID"].ToString(),
                CustomField1 = row["Custom Field 1"].ToString(),
                TradeDate = row["Trade Date"].TryParse<DateOnly>(),
                FXMPTradeID = row["FXMP Trade ID"].ToString(),
                NatWestSchemeID = row["NatWest Scheme ID"].ToString(),
                NatWestSchemeName = row["NatWest Scheme Name"].ToString(),
                GFXCounterparty = row["GFX Counterparty"].ToString(),
                ValueDate = row["Value Date"].TryParse<DateOnly>(),
                CCY = row["CCY"].ToString(),
                Amount = row["Amount"].Parse<decimal>(),
                Rate = row["Rate"].TryParse<decimal>(out var rate) ? rate : null,
                PaymentDirection = row["Payment Direction"].ToString(),
                DealType = row["Deal Type"].ToString(),
                RBSReceiptDate = row["RBS Receipt Date"].TryParse<DateOnly>(),
                SubmittedCashflowID = row["Submitted Cashflow ID"].ToString(),
                Submitted = row["Submitted"].TryParse<DateTime>(),
                RBSPaymentRef = row["RBS Payment Ref"].ToString()
            };
        }

    }
}
