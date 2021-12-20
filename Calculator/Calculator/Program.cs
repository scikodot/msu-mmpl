using System;

namespace Calculator
{
    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Welcome to the Calculator!\nEnter 'q' to exit.\n");
            while (true)
            {
                Console.Write("Input: ");
                var input = Console.ReadLine();

                if (input.Trim() == "q")
                {
                    Console.WriteLine("Exiting...");
                    break;
                }

                try
                {
                    Console.WriteLine($"Result: {Calculate(input)}\n");
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine($"Error: {ae.Message}\n");
                }
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
