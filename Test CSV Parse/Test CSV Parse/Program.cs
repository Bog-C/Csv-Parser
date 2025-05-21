// See https://aka.ms/new-console-template for more information
//using nietras.SeparatedValues;

//Console.WriteLine("Hello, World!");

using CsvHelper;
using CsvHelper.Configuration;
using nietras.SeparatedValues;
using System.Diagnostics;
using System.Globalization;
using Transaction = Test_CSV_Parse.Transaction;

public class Program
{
    public static void Main()
    {
        string filePathForSep = "Cashflow Settlement Report for Sep.csv";
        string filePathForCsvHelper = "Cashflow Settlement Report for CsvHelper.csv";

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        CsvHelperParse(filePathForCsvHelper); // CsvHelper
        stopwatch.Stop();
        
        Console.WriteLine();
        Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine();


        stopwatch.Restart();
        SepParse(filePathForSep); // Sep
        stopwatch.Stop();

        Console.WriteLine();
        Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine();

        //GenerateDummyData(filePathForCsvHelper, 1000);
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
        using var csv = new CsvReader(reader, config);

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
                RowNo = csv.GetField("Row No"),
                DetailNo = csv.GetField("Detail No"),
                SchemeTransactionID = csv.GetField("Scheme Transaction ID"),
                CustomField1 = csv.GetField("Custom Field 1"),
                TradeDate = csv.GetField("Trade Date"),
                FXMPTradeID = csv.GetField("FXMP Trade ID"),
                NatWestSchemeID = csv.GetField("NatWest Scheme ID"),
                NatWestSchemeName = csv.GetField("NatWest Scheme Name"),
                GFXCounterparty = csv.GetField("GFX Counterparty"),
                ValueDate = csv.GetField("Value Date"),
                CCY = csv.GetField("CCY"),
                Amount = csv.GetField("Amount"),
                Rate = csv.GetField("Rate"),
                PaymentDirection = csv.GetField("Payment Direction"),
                DealType = csv.GetField("Deal Type"),
                RBSReceiptDate = csv.GetField("RBS Receipt Date"),
                SubmittedCashflowID = csv.GetField("Submitted Cashflow ID"),
                Submitted = csv.GetField("Submitted"),
                RBSPaymentRef = csv.GetField("RBS Payment Ref")
            };
            result.Add(row);
            //ConsoleWriteLine(row);
        }

        return result;
    }

    private static void SepParse(string filePath)
    {
        using var reader = Sep.Reader(o => o with { Unescape = true }).FromFile(filePath);

        var result = new List<StandardExcelRow>();
        

        foreach (var readRow in reader)
        {
            var row = MapRow(readRow);
            result.Add(row);
            //ConsoleWriteLine(row);
        }
    }

    

    private static StandardExcelRow MapRow(SepReader.Row row)
    {
        return new StandardExcelRow
        {
            RowNo = row["Row No"].ToString(),
            DetailNo = row["Detail No"].ToString(),
            SchemeTransactionID = row["Scheme Transaction ID"].ToString(),
            CustomField1 = row["Custom Field 1"].ToString(),
            TradeDate = row["Trade Date"].ToString(),
            FXMPTradeID = row["FXMP Trade ID"].ToString(),
            NatWestSchemeID = row["NatWest Scheme ID"].ToString(),
            NatWestSchemeName = row["NatWest Scheme Name"].ToString(),
            GFXCounterparty = row["GFX Counterparty"].ToString(),
            ValueDate = row["Value Date"].ToString(),
            CCY = row["CCY"].ToString(),
            Amount = row["Amount"].ToString(),
            Rate = row["Rate"].ToString(),
            PaymentDirection = row["Payment Direction"].ToString(),
            DealType = row["Deal Type"].ToString(),
            RBSReceiptDate = row["RBS Receipt Date"].ToString(),
            SubmittedCashflowID = row["Submitted Cashflow ID"].ToString(),
            Submitted = row["Submitted"].ToString(),
            RBSPaymentRef = row["RBS Payment Ref"].ToString()
        };
    }

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
                RowNo = (i + 1).ToString(),
                DetailNo = random.Next(1000, 9999).ToString(),
                SchemeTransactionID = Guid.NewGuid().ToString(),
                CustomField1 = $"Custom{i + 1}",
                TradeDate = DateTime.Now.AddDays(-random.Next(0, 30)).ToString("yyyy-MM-dd"),
                FXMPTradeID = $"FXMP{random.Next(10000, 99999)}",
                NatWestSchemeID = $"NWID{random.Next(1000, 9999)}",
                NatWestSchemeName = "NatWest Scheme",
                GFXCounterparty = $"Counterparty{random.Next(1, 10)}",
                ValueDate = DateTime.Now.AddDays(random.Next(1, 30)).ToString("yyyy-MM-dd"),
                CCY = "USD",
                Amount = (random.NextDouble() * 10000).ToString("F2"),
                Rate = (random.NextDouble() * 2).ToString("F4"),
                PaymentDirection = random.Next(0, 2) == 0 ? "IN" : "OUT",
                DealType = random.Next(0, 2) == 0 ? "Spot" : "Forward",
                RBSReceiptDate = DateTime.Now.AddDays(random.Next(1, 30)).ToString("yyyy-MM-dd"),
                SubmittedCashflowID = Guid.NewGuid().ToString(),
                Submitted = random.Next(0, 2) == 0 ? "Yes" : "No",
                RBSPaymentRef = $"RBS{random.Next(100000, 999999)}"
            };
            csv.WriteRecord(row);
            csv.NextRecord();
        }
    }
}