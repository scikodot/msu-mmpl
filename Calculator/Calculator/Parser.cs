﻿using System.Collections.Generic;
using System.Globalization;

namespace Calculator
{
    public class Parser
    {
        public IEnumerable<Token> Parse(string line)
        {
            string s = "";
            foreach (var c in line)
            {
                if (c == ' ')
                    continue;
                else if (int.TryParse(c.ToString(), out _) || c == '.')
                {
                    s += c;
                }
                else if (TokenOperation.ValidTokens.Contains(c))
                {
                    if (s != "")
                    {
                        yield return new TokenNumber(double.Parse(s, CultureInfo.InvariantCulture));
                        s = "";
                    }

                    yield return TokenOperation.Create(c.ToString());
                }
            }

            if (s != "")
                yield return new TokenNumber(double.Parse(s, CultureInfo.InvariantCulture));
        }
    }
}
