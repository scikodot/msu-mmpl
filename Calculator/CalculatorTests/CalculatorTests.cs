using System;

using Xunit;

using Calculator;

namespace CalculatorTests
{
    public class CalculatorTests
    {
        [Theory]
        [MemberData(nameof(CalculatorTestsData.InputData), MemberType = typeof(CalculatorTestsData))]
        public void Test(string input, string expected)
        {
            try
            {
                double inputNumeric = Program.Calculate(input);
                Assert.True(double.TryParse(expected, out double expectedNumeric));
                Assert.Equal(expectedNumeric, inputNumeric);
            }
            catch (ArgumentException)  // Avoid catching assertion exceptions
            {
                Assert.Equal("ex", expected);
            }
        }
    }
}
