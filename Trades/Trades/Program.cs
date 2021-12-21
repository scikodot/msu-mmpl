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
        private static readonly string _filename = Environment.CurrentDirectory + "/Resources/trades.zip";
        private static string[] _keys;

        static void Main()
        {
            using var zip = ZipFile.OpenRead(_filename);
            using var stream = zip.GetEntry("trades.txt").Open();
            using var reader = new StreamReader(stream);

            // Get dict keys
            _keys = Enumerate(reader).First().Split('\t');

            // Get board related entries
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

            Console.WriteLine("Luckiest stocks:\n");
            Console.WriteLine(" SecurityCode | OpeningPrice | ClosingPrice | Increase | Percentage | Volume ");
            Console.WriteLine("-----------------------------------------------------------------------------");
            foreach (var stock in stocks.Take(10))
            {
                double increase = stock.ClosingPrice - stock.OpeningPrice;
                Console.WriteLine(
                    $" {stock.SecurityCode,-12} |" +
                    $" {stock.OpeningPrice,-12} |" +
                    $" {stock.ClosingPrice,-12} |" +
                    $" {increase,-8} |" +
                    $" {(increase >= 0 ? "+" : "") + $"{increase / stock.OpeningPrice:f5}%",-10} |" +
                    $" {stock.TotalVolume,-6} ");
            }

            Console.WriteLine();

            Console.WriteLine("Unluckiest stocks:\n");
            Console.WriteLine(" SecurityCode | OpeningPrice | ClosingPrice | Increase | Percentage | Volume ");
            Console.WriteLine("-----------------------------------------------------------------------------");
            foreach (var stock in stocks.TakeLast(10))
            {
                double increase = stock.ClosingPrice - stock.OpeningPrice;
                Console.WriteLine(
                    $" {stock.SecurityCode,-12} |" +
                    $" {stock.OpeningPrice,-12} |" +
                    $" {stock.ClosingPrice,-12} |" +
                    $" {increase,-8} |" +
                    $" {(increase >= 0 ? "+" : "") + $"{increase / stock.OpeningPrice:f5}%",-10} |" +
                    $" {stock.TotalVolume,-6} ");
            }
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
    }
}
