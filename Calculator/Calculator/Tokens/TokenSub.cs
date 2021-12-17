namespace Calculator
{
    public class TokenSub : TokenOperation
    {
        public TokenSub()
        {
            _associativity = Associativity.Left;
            _precedence = 2;
        }
    }
}
