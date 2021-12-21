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
        private static readonly string _filename = 
            Environment.CurrentDirectory + "/Resources/trades.zip";
        private static string[] _keys;

        static void Main()
        {
            SetKeys();
            DescribeBoard("TQBR");
            DescribeBoard("FQBR");
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

        private static void Print(IEnumerable<Dictionary<string, string>> lines, int count = 5)
        {
            using var enumerator = lines.GetEnumerator();
            enumerator.MoveNext();

            for (int i = 0; i < count; i++)
            {
                foreach (var kv in enumerator.Current)
                {
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
                }
                Console.WriteLine();

                enumerator.MoveNext();
            }
        }

        private static void SetKeys()
        {
            using var zip = ZipFile.OpenRead(_filename);
            using var stream = zip.GetEntry("trades.txt").Open();
            using var reader = new StreamReader(stream);

            _keys = Enumerate(reader).First().Split('\t');
        }

        private static void DescribeBoard(string board)
        {
            using var zip = ZipFile.OpenRead(_filename);
            using var stream = zip.GetEntry("trades.txt").Open();
            using var reader = new StreamReader(stream);

            // Get board related entries
            var trades = Enumerate(reader)
                .Select(x => Split(x))
                .Where(x => x["SECBOARD"] == board);

            // Get opening and closing prices
            (double open, double close) = OpenClosePrice(trades);
            Console.WriteLine($"{board} description:");
            Console.WriteLine($"Opening price: {open}");
            Console.WriteLine($"Closing price: {close}");
            Console.WriteLine();

            //Print(trades);
        }

        private static (double, double) OpenClosePrice(IEnumerable<Dictionary<string, string>> trades)
        {
            // Trades are sorted by time
            return (double.Parse(trades.First()["PRICE"], CultureInfo.InvariantCulture), 
                    double.Parse(trades.Last()["PRICE"], CultureInfo.InvariantCulture));
        }
    }
}
