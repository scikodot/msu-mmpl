namespace Calculator
{
    public class TokenNumber : Token
    {
        public double Value { get; set; }

        public TokenNumber(double value)
        {
            Value = value;
        }
    }
}
