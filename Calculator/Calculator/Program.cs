using System;

namespace Calculator
{
    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Input:");
            var input = Console.ReadLine();

            try
            {
                Console.WriteLine($"Result: {Calculate(input)}");
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
            }
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
