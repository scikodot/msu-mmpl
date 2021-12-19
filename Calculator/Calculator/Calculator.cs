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
                        (x, y) = GetArgs(stack);
                        stack.Push(y + x);
                        break;
                    case TokenSub sub:
                        (x, y) = GetArgs(stack);
                        stack.Push(y - x);
                        break;
                    case TokenMul mul:
                        (x, y) = GetArgs(stack);
                        stack.Push(y * x);
                        break;
                    case TokenDiv div:
                        (x, y) = GetArgs(stack);
                        stack.Push(y / x);
                        break;
                    case TokenPow pow:
                        (x, y) = GetArgs(stack);
                        stack.Push(Math.Pow(y, x));
                        break;
                    default:
                        throw new ArgumentException($"Invalid token: {token}");
                }
            };

            if (stack.TryPop(out double top) && stack.Count == 0)
                return top;
            else
                throw new ArgumentException("Invalid expression");
        }

        private (double, double) GetArgs(Stack<double> stack)
        {
            if (stack.TryPop(out double x) && stack.TryPop(out double y))
                return (x, y);
            else
                throw new ArgumentException("Invalid expression");
        }
    }
}
