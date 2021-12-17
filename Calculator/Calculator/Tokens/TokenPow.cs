namespace Calculator
{
    public class TokenPow : TokenOperation
    {
        public TokenPow()
        {
            _associativity = Associativity.Right;
            _precedence = 4;
        }
    }
}
