namespace Calculator
{
    public class TokenMul : TokenOperation
    {
        public TokenMul()
        {
            _associativity = Associativity.Left;
            _precedence = 3;
        }
    }
}
