using System;
using System.Collections.Generic;

namespace Calculator
{
    public class Calculator
    {
        public double Calculate(IEnumerable<Token> tokens)
        {
            var stack = new Stack<double>();
            foreach (var token in tokens)
            {
                double x, y;
                switch (token)
                {
                    case TokenNumber num:
                        stack.Push(num.Value);
                        break;
                    case TokenAdd add:
                        x = stack.Pop();
                        y = stack.Pop();
                        stack.Push(y + x);
                        break;
                    case TokenSub sub:
                        x = stack.Pop();
                        y = stack.Pop();
                        stack.Push(y - x);
                        break;
                    case TokenMul mul:
                        x = stack.Pop();
                        y = stack.Pop();
                        stack.Push(y * x);
                        break;
                    case TokenDiv div:
                        x = stack.Pop();
                        y = stack.Pop();
                        stack.Push(y / x);
                        break;
                    case TokenPow pow:
                        x = stack.Pop();
                        y = stack.Pop();
                        stack.Push(Math.Pow(y, x));
                        break;
                    default:
                        throw new ArgumentException($"Unknown token: {token}");
                }
            };

            return stack.Pop();
        }
    }
}
