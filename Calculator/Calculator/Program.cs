using System;

namespace Calculator
{
    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Input:");
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("No input is supplied.");
                return;
            }            

            Console.WriteLine($"Result: {Calculate(input)}");
        }

        public static double Calculate(string input)
        {
            var parser = new Parser();
            var tokens = parser.Parse(input);

            var yard = new ShuntingYard();
            var reversePolish = yard.Process(tokens);

            var calculator = new Calculator();
            return calculator.Calculate(reversePolish);
        }
    }
}
