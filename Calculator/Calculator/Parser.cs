using System;
using System.Collections.Generic;
namespace Calculator
{
    public class Parser
    {
        public IEnumerable<Token> Parse(string line)
        {
            string s = "";
            foreach (var c in line)
            {
                if (int.TryParse(c.ToString(), out _) || c == '.')
                {
                    s += c;
                }
                else if (TokenOperation.ValidTokens.Contains(c))
                {
                    if (s != "")
                        yield return new TokenNumber(double.Parse(s));

                    yield return TokenOperation.Create(c.ToString());
                }
            }
        }
    }
}
