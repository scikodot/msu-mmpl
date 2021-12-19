using System;
using System.Collections.Generic;

namespace Calculator
{
    public class ShuntingYard
    {
        public IEnumerable<Token> Process(IEnumerable<Token> tokens)
        {
            var stack = new Stack<TokenOperation>();
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case TokenNumber num:
                        yield return token;
                        break;
                    case TokenOperation oper:
                        TokenOperation top;
                        switch (oper)
                        {
                            case TokenLeftPar:
                                stack.Push(token as TokenOperation);
                                break;
                            case TokenRightPar:
                                bool lparFound = false;
                                while (stack.TryPop(out top))
                                {
                                    if (top is TokenLeftPar)
                                    {
                                        lparFound = true;
                                        break;
                                    }
                                    yield return top;
                                }

                                if (!lparFound)
                                    throw new ArgumentException("Unbalanced parentheses (left missing)");

                                break;
                            case TokenAdd:
                            case TokenSub:
                            case TokenMul:
                            case TokenDiv:
                            case TokenPow:
                                if (stack.TryPeek(out top) && 
                                    !(top is TokenLeftPar) &&
                                    (oper.Precedence < top.Precedence ||
                                     (oper.Precedence == top.Precedence && 
                                      oper.Associativity == Associativity.Left)))
                                    yield return stack.Pop();
                                stack.Push(oper);
                                break;
                            default:
                                throw new ArgumentException($"Invalid token: {oper}");
                        }
                        break;
                    default:
                        throw new ArgumentException($"Invalid token: {token}");
                }
            }

            // Clear stack
            while (stack.TryPop(out TokenOperation top))
            {
                if (top is TokenLeftPar)
                    throw new ArgumentException($"Unbalanced parentheses (right missing)");
                else
                    yield return top;
            }
        }
    }
}
