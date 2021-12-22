using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Trades
{
    class Program
    {
        private static readonly string _inputFile = 
            Environment.CurrentDirectory + "/Resources/trades.zip";
        private static readonly string _outputFile = 
            Environment.CurrentDirectory + "/output/output.txt";
        private static string[] _keys;

        static void Main()
        {
            using var zip = ZipFile.OpenRead(_inputFile);
            using var stream = zip.GetEntry("trades.txt").Open();
            using var reader = new StreamReader(stream);

            // Get dict keys
            _keys = Enumerate(reader).First().Split('\t');

            var trades = Enumerate(reader)
                .Select(x => Split(x))
                .Where(x => x["SECBOARD"] == "TQBR" || x["SECBOARD"] == "FQBR")
                .GroupBy(x => x["SECCODE"],
                    x => (double.Parse(x["PRICE"], CultureInfo.InvariantCulture), 
                          double.Parse(x["VOLUME"], CultureInfo.InvariantCulture)),
                    (k, v) => new
                    {
                        SecurityCode = k,
                        OpeningPrice = v.First().Item1,
                        ClosingPrice = v.Last().Item1,
                        TotalVolume = v.Sum(x => x.Item2)
                    })
                .OrderByDescending(x => x.ClosingPrice - x.OpeningPrice);
            
            var stocks = trades.ToList();

            string luckiestStocks = StocksTable("LuckiestStocks", stocks.Take(10));
            string unluckiestStocks = StocksTable("UnluckiestStocks", stocks.TakeLast(10));

            // Output to console
            Console.WriteLine(luckiestStocks);
            Console.WriteLine(unluckiestStocks);

            // Output to file
            Directory.CreateDirectory(Path.GetDirectoryName(_outputFile));
            File.WriteAllText(_outputFile, string.Join("\n", luckiestStocks, unluckiestStocks));
        }

        private static IEnumerable<string> Enumerate(StreamReader reader)
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                yield return line;
                line = reader.ReadLine();
            }
        }

        private static Dictionary<string, string> Split(string line)
        {
            return new Dictionary<string, string>(_keys.Zip(line.Split('\t'),
                (x, y) => KeyValuePair.Create(x, y)));
        }

        private static string StocksTable(string header, IEnumerable<dynamic> stocks)
        {
            int leftPadding = (77 - header.Length) / 2;
            int rightPadding = 77 - header.Length - leftPadding;
            string headerCentered = 
                new string('~', leftPadding) + header + new string('~', rightPadding);

            string table = headerCentered + "\n" +
                "-----------------------------------------------------------------------------\n" +
                " SecurityCode | OpeningPrice | ClosingPrice | Increase | Percentage | Volume \n" +
                "-----------------------------------------------------------------------------\n";
            foreach (var stock in stocks)
            {
                double increase = stock.ClosingPrice - stock.OpeningPrice;
                table +=
                    $" {stock.SecurityCode,-12} |" +
                    $" {stock.OpeningPrice,-12} |" +
                    $" {stock.ClosingPrice,-12} |" +
                    $" {increase,-8} |" +
                    $" {(increase > 0 ? "+" : "") + $"{increase / stock.OpeningPrice:f5}%",-10} |" +
                    $" {stock.TotalVolume,-6} \n";
            }

            return table;
        }
    }
}
