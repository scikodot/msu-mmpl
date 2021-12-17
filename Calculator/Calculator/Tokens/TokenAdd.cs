namespace Calculator
{
    public class TokenAdd : TokenOperation
    {
        public TokenAdd()
        {
            _associativity = Associativity.Left;
            _precedence = 2;
        }
    }
}
