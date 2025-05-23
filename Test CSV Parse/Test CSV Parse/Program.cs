// See https://aka.ms/new-console-template for more information
//using nietras.SeparatedValues;

//Console.WriteLine("Hello, World!");

using CsvHelper;
using CsvHelper.Configuration;
using nietras.SeparatedValues;
using RecordParser.Builders.Reader;
using RecordParser.Extensions;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using SoftCircuits.CsvParser;
using Transaction = Test_CSV_Parse.Transaction;
using CsvWriter = CsvHelper.CsvWriter;
using System;
using BenchmarkDotNet.Running;
using CommandLine;

namespace Test_CSV_Parse
{
    public class Program
    {
        public static void Main()
        {
            #region benchmark
            //BenchmarkRunner.Run<Benchmark>();
            #endregion benchmark

            //string filePathForSep = "Cashflow Settlement Report for Sep - 1k lines.csv";
            string filePathForSep = "New Cashflow Settlement Report Sample.csv";
            //string filePathForCsvHelper = "Cashflow Settlement Report for CsvHelper.csv";
            //string filePathForRecordParser = "Cashflow Settlement Report for RecordParser.csv";

            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePathForSep);

            var result = Parse(reader, row => !row.DetailNo.HasValue).ToList();

            foreach (var group in result)
            {
                Console.WriteLine($"TOTAL: {group.Key} ");
                foreach (var item in group)
                {
                    ConsoleWriteLine(item);
                }
            }

            #region test sep different versions of parse methods

            //Stopwatch stopwatch = new Stopwatch();

            //stopwatch.Start();
            ////SepParse(filePathForSep); // Sep

            ////SepParseWithLoggingV1(filePathForSep); 
            ////SepParseWithLoggingV2(filePathForSep);
            //SepParseWithLoggingV3(filePathForSep); // Sep with logging
            //stopwatch.Stop();

            //Console.WriteLine();
            //Console.WriteLine($"Elapsed time Sep v3: {stopwatch.ElapsedMilliseconds} ms");
            //Console.WriteLine();

            #endregion test sep different versions of parse methods

            #region other libraries
            //stopwatch.Start();
            ////CsvHelperParse(filePathForCsvHelper); // CsvHelper
            //stopwatch.Stop();

            //Console.WriteLine();
            //Console.WriteLine($"Elapsed time CsvHelper: {stopwatch.ElapsedMilliseconds} ms");
            //Console.WriteLine();


            //stopwatch.Restart();
            //RecordParser(filePathForRecordParser); // RecordParser
            //stopwatch.Stop();

            //Console.WriteLine();
            //Console.WriteLine($"Elapsed time RecordParser: {stopwatch.ElapsedMilliseconds} ms");
            //Console.WriteLine();


            //stopwatch.Restart();
            //SoftCircuitsParser(filePathForRecordParser); // SoftCircuits
            //stopwatch.Stop();

            //Console.WriteLine();
            //Console.WriteLine($"Elapsed time SoftCircuits: {stopwatch.ElapsedMilliseconds} ms");
            //Console.WriteLine();
            #endregion other libraries

            ////GenerateDummyData(filePathForCsvHelper, 10000);
        }

        private static IEnumerable<IGrouping<decimal, StandardExcelRow>> Parse(SepReader reader, Func<StandardExcelRow, bool> isTotalRow)
        {
            decimal totalAmount = 0;
            var accumulator = new List<StandardExcelRow>();

            foreach (var readRow in reader)
            {
                
                var row = MapRow(readRow); // implemented with TryParse<T> !!!

                if (!isTotalRow(row))
                {
                    totalAmount += row.Amount;

                    accumulator.Add(row);
                }
                else
                {

                    if (totalAmount == row.Amount && accumulator.Count > 0)
                    {
                        yield return new Grouping<decimal, StandardExcelRow>(row.Amount, accumulator.ToList());
                    }

                    totalAmount = 0;
                    accumulator.Clear();
                }
            }
        }


        #region sep methods

        private static void SepParseWithLoggingV3(string filePath)
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

            decimal batchTotal = 0;

            var batchIndex = 1;

            foreach (var readRow in reader)
            {
                var row = MapRow(readRow);
                
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

        private static void SepParseWithLoggingV2(string filePath)
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

            decimal batchTotal = 0;

            var batchIndex = 1;

            foreach (var readRow in reader)
            {
                if (!readRow[1].TryParse<int>(out int val))
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

        private static void SepParseWithLoggingV1(string filePath)
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
                RBSReceiptDate = row["NatWest Receipt Date"].TryParse<DateOnly>(),
                //SubmittedCashflowID = row["Submitted Cashflow ID"].ToString(),
                Submitted = row["Submitted"].TryParse<DateTime>(),
                RBSPaymentRef = row["NatWest Internal Booking Ref"].ToString()
            };
        }

        #endregion sep methods


        #region other methods

        private static List<StandardExcelRow> SoftCircuitsParser(string filePath)
        {
            List<StandardExcelRow> results = new();

            using (CsvReader<StandardExcelRow> reader = new(filePath))
            {
                // Read header and use to determine column order
                reader.ReadHeaders(true);
                // Read data
                StandardExcelRow? row;
                while ((row = reader.Read()) != null)
                    results.Add(row);
            }

            return results;

            //while (reader.Read())
            //Console.WriteLine(string.Join(", ", reader.Columns));
        }


        private static List<StandardExcelRow> RecordParser(string filePath)
        {
            using TextReader textReader = new StreamReader(filePath);

            var reader = new VariableLengthReaderBuilder<StandardExcelRow>()
                .Map(x => x.RowNo, 0)
                .Map(x => x.DetailNo, 1)
                .Map(x => x.SchemeTransactionID, 2)
                .Map(x => x.CustomField1, 3)
                .Map(x => x.TradeDate, 4)
                .Map(x => x.FXMPTradeID, 5)
                .Map(x => x.NatWestSchemeID, 6)
                .Map(x => x.NatWestSchemeName, 7)
                .Map(x => x.GFXCounterparty, 8)
                .Map(x => x.ValueDate, 9)
                .Map(x => x.CCY, 10)
                .Map(x => x.Amount, 11)
                .Map(x => x.Rate, 12)
                .Map(x => x.PaymentDirection, 13)
                .Map(x => x.DealType, 14)
                .Map(x => x.RBSReceiptDate, 15)
                .Map(x => x.SubmittedCashflowID, 16)
                .Map(x => x.Submitted, 17)
                .Map(x => x.RBSPaymentRef, 18)
                .Build(",");

            var readOptions = new VariableLengthReaderOptions
            {
                HasHeader = true,
                ContainsQuotedFields = false,
                ParallelismOptions = new()
                {
                    Enabled = true,
                    EnsureOriginalOrdering = true,
                    MaxDegreeOfParallelism = 4
                }
            };

            var result = new List<StandardExcelRow>();

            var records = textReader.ReadRecords(reader, readOptions);

            foreach (var r in records)
                result.Add(r);
            //ConsoleWriteLine(r);

            return result;
        }


        private static List<StandardExcelRow> CsvHelperParse(string filePath)
        {
            var result = new List<StandardExcelRow>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvHelper.CsvReader(reader, config);

            // Read header to get expected field count
            csv.Read();
            csv.ReadHeader();
            var header = csv.HeaderRecord;
            int expectedColCount = header.Length;

            while (csv.Read())
            {
                // Skip rows with "TOTAL:" in any field or wrong column count
                var rowFields = new string[expectedColCount];
                for (int i = 0; i < expectedColCount; i++)
                {
                    rowFields[i] = csv.GetField(i) ?? string.Empty;
                }
                //if (rowFields.Any(f => f.Trim().Equals("TOTAL:", StringComparison.OrdinalIgnoreCase)))
                //    continue;
                //if (csv.Parser.Count != expectedColCount)
                //    continue;

                var row = new StandardExcelRow
                {
                    RowNo = csv.GetField<int>("Row No"),
                    DetailNo = csv.GetField<int>("Detail No"),
                    SchemeTransactionID = csv.GetField("Scheme Transaction ID"),
                    CustomField1 = csv.GetField("Custom Field 1"),
                    TradeDate = csv.GetField<DateOnly>("Trade Date"),
                    FXMPTradeID = csv.GetField("FXMP Trade ID"),
                    NatWestSchemeID = csv.GetField("NatWest Scheme ID"),
                    NatWestSchemeName = csv.GetField("NatWest Scheme Name"),
                    GFXCounterparty = csv.GetField("GFX Counterparty"),
                    ValueDate = csv.GetField<DateOnly>("Value Date"),
                    CCY = csv.GetField("CCY"),
                    Amount = csv.GetField<decimal>("Amount"),
                    Rate = csv.GetField<decimal>("Rate"),
                    PaymentDirection = csv.GetField("Payment Direction"),
                    DealType = csv.GetField("Deal Type"),
                    RBSReceiptDate = csv.GetField<DateOnly>("NatWest Receipt Date"),
                    //SubmittedCashflowID = csv.GetField("Submitted Cashflow ID"),
                    Submitted = csv.GetField<DateTime>("Submitted"),
                    RBSPaymentRef = csv.GetField("NatWest Internal Booking Ref")
                };
                result.Add(row);
                //ConsoleWriteLine(row);
            }

            return result;
        }

        //private static List<StandardExcelRow> SepParse(string filePath)
        //{
        //    using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

        //    var result = new List<StandardExcelRow>();


        //    foreach (var readRow in reader)
        //    {
        //        var row = MapRow(readRow);
        //        result.Add(row);
        //        //ConsoleWriteLine(row);
        //    }

        //    return result;
        //}

        private static void ConsoleWriteLine(Transaction detail)
        {
            Console.Write($"{detail.TransactionId} "); ;
            Console.Write($"{detail.CustomField} "); ;
            Console.Write($"{detail.TradeDate} "); ;
            Console.Write($"{detail.Currency} "); ;
            Console.Write($"{detail.Amount} "); ;
            Console.Write($"{detail.Rate} "); ;
            Console.Write($"{detail.Type} "); ;
        }

        private static void ConsoleWriteLine(StandardExcelRow detail)
        {
            Console.Write($"{detail.RowNo} ");
            Console.Write($"{detail.DetailNo} ");
            Console.Write($"{detail.SchemeTransactionID} ");
            Console.Write($"{detail.CustomField1} ");
            Console.Write($"{detail.TradeDate} ");
            Console.Write($"{detail.FXMPTradeID} ");
            Console.Write($"{detail.NatWestSchemeID} ");
            Console.Write($"{detail.NatWestSchemeName} ");
            Console.Write($"{detail.GFXCounterparty} ");
            Console.Write($"{detail.ValueDate} ");
            Console.Write($"{detail.CCY} ");
            Console.Write($"{detail.Amount} ");
            Console.Write($"{detail.Rate} ");
            Console.Write($"{detail.PaymentDirection} ");
            Console.Write($"{detail.DealType} ");
            Console.Write($"{detail.RBSReceiptDate} ");
            Console.Write($"{detail.SubmittedCashflowID} ");
            Console.Write($"{detail.Submitted} ");
            Console.Write($"{detail.RBSPaymentRef} ");
            Console.WriteLine();
        }

        private static void GenerateDummyData(string filePath, int rowCount)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(filePath) || new FileInfo(filePath).Length == 0
            };

            using var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);

            // Write header if file is empty
            if (config.HasHeaderRecord)
            {
                csv.WriteHeader<StandardExcelRow>();
                csv.NextRecord();
            }

            var random = new Random();
            for (int i = 0; i < rowCount; i++)
            {
                var row = new StandardExcelRow
                {
                    RowNo = (i + 1),
                    DetailNo = random.Next(1000, 9999),
                    SchemeTransactionID = Guid.NewGuid().ToString(),
                    CustomField1 = $"Custom{i + 1}",
                    TradeDate = DateOnly.FromDayNumber(-random.Next(0, 30)),
                    FXMPTradeID = $"FXMP{random.Next(10000, 99999)}",
                    NatWestSchemeID = $"NWID{random.Next(1000, 9999)}",
                    NatWestSchemeName = "NatWest Scheme",
                    GFXCounterparty = $"Counterparty{random.Next(1, 10)}",
                    ValueDate = DateOnly.FromDayNumber(random.Next(1, 30)),
                    CCY = "USD",
                    Amount = decimal.Parse((random.NextDouble() * 10000).ToString("F2")),
                    Rate = decimal.Parse((random.NextDouble() * 2).ToString("F4")),
                    PaymentDirection = random.Next(0, 2) == 0 ? "IN" : "OUT",
                    DealType = random.Next(0, 2) == 0 ? "Spot" : "Forward",
                    RBSReceiptDate = DateOnly.FromDayNumber(random.Next(1, 30)),
                    SubmittedCashflowID = Guid.NewGuid().ToString(),
                    Submitted = DateTime.Now.AddDays(random.Next(1, 30)),
                    RBSPaymentRef = $"RBS{random.Next(100000, 999999)}"
                };
                csv.WriteRecord(row);
                csv.NextRecord();
            }
        }

        #endregion other methods
    }
}