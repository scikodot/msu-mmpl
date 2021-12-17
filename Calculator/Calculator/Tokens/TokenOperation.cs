using System;
using System.Collections.Generic;

namespace Calculator
{
    public class TokenOperation : Token
    {
        private static readonly List<char> _validTokens = new() { '+', '-', '*', '/', '^', '(', ')' };
        public static List<char> ValidTokens => _validTokens;

        public static Token Create(string s)
        {
            return s switch
            {
                "+" => new TokenAdd(),
                "-" => new TokenSub(),
                "*" => new TokenMul(),
                "/" => new TokenDiv(),
                "^" => new TokenPow(),
                "(" => new TokenLeftPar(),
                ")" => new TokenRightPar(),
                _ => throw new ArgumentException($"Unsupported operation: {s}"),
            };
        }
    }
}
