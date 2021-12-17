using System;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input:");
            var input = Console.ReadLine();

            var parser = new Parser();
            var tokens = parser.Parse(input);

            var yard = new ShuntingYard();
            var reversePolish = yard.Process(tokens);

            var calculator = new Calculator();
            var result = calculator.Calculate(reversePolish);
            Console.WriteLine($"Result: {result}");
        }
    }
}
