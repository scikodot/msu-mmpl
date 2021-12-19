using System;
using System.Collections.Generic;
using System.Globalization;

namespace Calculator
{
    public class Parser
    {
        public IEnumerable<Token> Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException("Empty input");

            string s = "";
            foreach (var c in line)
            {
                
                if (int.TryParse(c.ToString(), out _) || c == '.')
                {
                    s += c;
                }
                else
                {
                    if (s != "")
                    {
                        yield return ParseNumber(s);
                        s = "";
                    }

                    if (c == ' ')
                        continue;
                    else if (TokenOperation.ValidTokens.Contains(c))
                        yield return TokenOperation.Create(c.ToString());
                    else
                        throw new ArgumentException($"Invalid token: {c}");
                }                
            }

            if (s != "")
                yield return ParseNumber(s);
        }

        private TokenNumber ParseNumber(string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double res))
                return new TokenNumber(res);
            else
                throw new ArgumentException($"Invalid number: {s}");
        }
    }
}
