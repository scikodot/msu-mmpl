using System;
using System.Collections.Generic;

namespace CalculatorTests
{
    public class CalculatorTestsData
    {
        public static readonly IEnumerable<object[]> _inputData =
            new List<object[]>()
            {
                // Simple
                new object[] { "3+4", 7.ToString() },
                // Precedence/associativity stacking order
                new object[] { "2+3-4+5", (2 + 3 - 4 + 5).ToString() },
                new object[] { "2^2^2^2", Math.Pow(2, Math.Pow(2, Math.Pow(2, 2))).ToString() },
                new object[] { "2*3+4*5", (2 * 3 + 4 * 5).ToString() },
                new object[] { "2^3+4^5", (Math.Pow(2, 3) + Math.Pow(4, 5)).ToString() },
                // Single pars
                new object[] { "(2+3)/4", ((2 + 3) / 4.0).ToString() },
                new object[] { "2^(3+4)", Math.Pow(2, 3 + 4).ToString() },
                new object[] { "2-(3-4)", (2 - (3 - 4)).ToString() },
                new object[] { "(2^3)^4", Math.Pow(Math.Pow(2, 3), 4).ToString() },
                // Double pars
                new object[] { "2-(3^(4+5))", (2 - Math.Pow(3, 4 + 5)).ToString() },
                new object[] { "2-((3+4)/5)", (2 - ((3 + 4) / 5.0)).ToString() },
                // Double values
                new object[] { "1.23+4.56", (1.23 + 4.56).ToString() },
                new object[] { "1.23/(5/4)*6.7-8.901*2.3456", 
                    (1.23 / (5 / 4.0) * 6.7 - 8.901 * 2.3456).ToString() },
                // Complex
                new object[] { "3+4*2.567/(1-5.43)^2^3.012", 
                    (3 + 4 * 2.567 / Math.Pow(1 - 5.43, Math.Pow(2, 3.012))).ToString() },
            };
        public static IEnumerable<object[]> InputData => _inputData;
    }
}
