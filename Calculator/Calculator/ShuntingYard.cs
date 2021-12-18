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
                            case TokenLeftPar lpar:
                                stack.Push(token as TokenOperation);
                                break;
                            case TokenRightPar rpar:
                                while (stack.Count > 0)
                                {
                                    top = stack.Pop();
                                    if (top is TokenLeftPar)
                                        break;
                                    yield return top;
                                }
                                break;
                            default:
                                if (stack.TryPeek(out top) && 
                                    !(top is TokenLeftPar) &&
                                    (oper.Precedence < top.Precedence ||
                                     (oper.Precedence == top.Precedence && 
                                      oper.Associativity == Associativity.Left)))
                                    yield return stack.Pop();
                                stack.Push(oper);
                                break;
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unknown token: {token}");
                }
            }

            // Clear stack
            while (stack.TryPop(out TokenOperation oper))
                yield return oper;
        }
    }
}
