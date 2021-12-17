namespace Calculator
{
    public class TokenDiv : TokenOperation
    {
        public TokenDiv()
        {
            _associativity = Associativity.Left;
            _precedence = 3;
        }
    }
}
