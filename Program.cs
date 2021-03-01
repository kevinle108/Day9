using System;
using System.Collections.Generic;
using System.Linq;


namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                List<string> infix = InfixListFromUser();
                //infix.ForEach(token => Console.WriteLine(TokenType(token)));
                if (IsValidExpression(infix))
                {
                    List<string> postfix = PostFixFromInfix(infix);
                    DisplayList("Postfix Queue:", postfix);
                    Console.WriteLine($"The answer is {ValueOfPostfixList(postfix)}\n");
                }
                else Console.WriteLine("Please try again.");
                Console.WriteLine();
            }

            /// FUNCTIONS ///
            static List<string> TokensFromString(string expression)
            {
                List<string> tokensList = new List<string>();
                string token = "";
                bool insideNum = false;
                for (int i = 0; i < expression.Length; i++)
                {
                    char cur = expression[i];
                    if (CharType(cur) == "digit" || CharType(cur) == "decimal")
                    {
                        insideNum = true;
                        token += cur;
                        continue;
                    }
                    else
                    {
                        if (CharType(cur) == "space" && insideNum)
                        {
                            tokensList.Add(token);
                            token = "";
                            insideNum = false;
                            continue;
                        }
                        if (CharType(cur) == "space" && !insideNum) continue;
                        if (insideNum)
                        {
                            tokensList.Add(token);
                            token = "";
                            insideNum = false;
                        }
                        tokensList.Add(cur.ToString());
                    }
                }
                if (token != "")
                {
                    tokensList.Add(token);
                }
                return tokensList;
            }

            static string CharType(char c)
            {
                if (c == ' ') return "space";
                else if (c == '+' || c == '-' || c == '–' || c == '*' || c == '/' || c == '^' || c == '%') return "operand";
                else if (c == '(' || c == ')' || c == '{' || c == '}' || c == '[' || c == ']') return "parenthesis";
                else if (c == '.') return "decimal";
                else return "digit";
            }

            static void OutputTokens(List<string> tokens)
            {
                tokens.ForEach(token => Console.WriteLine(token));
            }

            static List<string> PostFixFromInfix(List<string> infixList)
            {
                List<string> postfixList = new List<string>();
                Stack<string> opStack = new Stack<string>();
                for (int i = 0; i < infixList.Count; i++)
                {
                    string cur = infixList[i];
                    if (double.TryParse(cur, out double token))
                    {
                        postfixList.Add(cur);
                    }
                    else
                    {
                        if (cur == "(" || cur == "[" || cur == "{")
                        {
                            opStack.Push(cur);
                            continue;
                        }
                        if (cur == ")" || cur == "}" || cur == "]")
                        {
                            string openParenthesis = matchingParenthesis(cur);
                            bool emptyStack = opStack.Count == 0;
                            while (!emptyStack && opStack.Peek() != openParenthesis)
                            {
                                postfixList.Add(opStack.Pop());
                                if (opStack.Count == 0) emptyStack = true;
                            }
                            if (emptyStack) return new List<string>();
                            opStack.Pop(); // to remove the open parenthesis from the stack
                            continue;
                        }
                        while (PrevOpShouldBeExecutedBeforeCurOp(opStack, cur))
                        {
                            postfixList.Add(opStack.Pop());
                        }
                        opStack.Push(cur);
                    }
                }
                while (opStack.Count != 0)
                {
                    postfixList.Add(opStack.Pop());
                }
                return postfixList;
            }

            static bool PrevOpShouldBeExecutedBeforeCurOp(Stack<string> opStack, string curOp)
            {
                if (opStack.Count < 1) return false;
                string prevOp = opStack.Peek();
                return PriorityValue(prevOp) >= PriorityValue(curOp);
            }

            static int PriorityValue(string op)
            {
                switch (op)
                {
                    case "^": return 3;

                    case "*":
                    case "/":
                    case "%":
                        return 2;

                    case "+":
                    case "-":
                        return 1;

                    default: // assumes that op is a (, [, or {
                        return 0;
                }
            }

            static string matchingParenthesis(string parenthesis)
            {
                switch (parenthesis)
                {
                    case ")": return "(";
                    case "]": return "[";
                    case "}": return "{";
                    default:
                        return "";
                }
            }

            static double OpResult(double num1, double num2, string op)
            {
                switch (op)
                {
                    case "^":
                        return Math.Pow(num1, num2);
                    case "*":
                        return num1 * num2;
                    case "/":
                        return num1 / num2;
                    case "%":
                        return num1 % num2;
                    case "+":
                        return num1 + num2;
                    case "-":
                        return num1 - num2;
                    default:
                        return -999999;
                }
            }

            static double ValueOfPostfixList(List<string> postfixList)
            {
                Stack<double> valueStack = new Stack<double>();
                string cur;
                for (int i = 0; i < postfixList.Count; i++)
                {
                    cur = postfixList[i];
                    if (double.TryParse(cur, out double num))
                    {
                        valueStack.Push(num);
                    }
                    else
                    {
                        // cur is an operator
                        if (valueStack.Count < 2) return -999.999;
                        double num2 = valueStack.Pop();
                        double num1 = valueStack.Pop();
                        double result = OpResult(num1, num2, cur);
                        if (result == -999999) return -999999;
                        valueStack.Push(result);
                    }
                }
                if (valueStack.Count != 1) return -999.999;
                return valueStack.Pop();
            }
            // for debugging purposes: 
            // a return of -999.999 means stack error
            // a return of -999999 means OpResult error

            static List<string> InfixListFromUser()
            {
                Console.Write("Enter a mathematical expression: ");
                List<string> infix = TokensFromString(Console.ReadLine());
                return infix;
            }

            static bool IsValidExpression(List<string> infixList)
            {
                if (!TokenListHasValidTypes(infixList)) return false;
                if (TokenType(infixList.First()) != "number" && TokenType(infixList.First()) != "open bracket")
                {
                    Console.WriteLine($"Invalid Expression! Expressions must begin with a number or opening bracket. Your expression begins with {infixList.First()}");
                    return false;
                }
                if (TokenType(infixList.Last()) != "number" && TokenType(infixList.Last()) != "close bracket")
                {
                    Console.WriteLine($"Invalid Expression! Expressions must end with a either a number or closing bracket. Your expression ends with {infixList.Last()}");
                    return false;
                }
                if (!HasMatchingBracketPairs(infixList)) return false;
                return true;
            }

            static string TokenType(string token)
            {
                string[] operators = { "+", "-", "*", "/", "%", "^" };
                string[] openbrackets = { "(", "{", "[" };
                string[] closebrackets = { ")", "}", "]" };
                if (double.TryParse(token, out double num)) return "number";
                if (operators.Any(token.Contains)) return "operator";
                if (openbrackets.Any(token.Contains)) return "open bracket";
                if (closebrackets.Any(token.Contains)) return "close bracket";
                return "invalid type";
            }

            static bool TokenListHasValidTypes(List<string> infixList)
            {
                List<string> invalids = new List<string>();
                for (int i = 0; i < infixList.Count; i++)
                {
                    if (TokenType(infixList[i]) == "invalid type")
                    {
                        Console.WriteLine($"Invalid Expression! {infixList[i]} is not a valid type. Please use only numbers, operators, and brackets in your expression");
                        invalids.Add(infixList[i]);
                    }
                    if (TokenType(infixList[i]) == "number" && i != 0)
                    {
                        if (TokenType(infixList[i - 1]) == "operator" || TokenType(infixList[i - 1]) == "open bracket") continue;
                        Console.WriteLine($"Invalid Expression! {infixList[i]} must be preceded by an operator or an opening bracket.");
                        return false;
                    }
                    if (TokenType(infixList[i]) == "operator")
                    {
                        if (TokenType(infixList[i - 1]) == "number" || TokenType(infixList[i - 1]) == "close bracket") continue;
                        Console.WriteLine($"Invalid Expression! {infixList[i]} must be preceded by a number or a closing bracket.");
                        return false;
                    }
                    if (TokenType(infixList[i]) == "open bracket" && i != 0)
                    {
                        if (TokenType(infixList[i - 1]) == "operator" || TokenType(infixList[i - 1]) == "open bracket") continue;
                        Console.WriteLine($"Invalid Expression! {infixList[i]} must be preceded by an operator or another opening bracket.");
                        return false;
                    }
                    if (TokenType(infixList[i]) == "close bracket")
                    {
                        if (TokenType(infixList[i - 1]) == "number" || TokenType(infixList[i - 1]) == "close bracket") continue;
                        Console.WriteLine($"Invalid Expression! {infixList[i]} must be preceded by a number or another closing bracket.");
                        return false;
                    }
                }
                if (invalids.Count != 0) return false;
                else return true;
            }

            static bool HasMatchingBracketPairs(List<string> infixList)
            {
                Stack<string> stack = new Stack<string>();
                for (int i = 0; i < infixList.Count; i++)
                {
                    if (TokenType(infixList[i]) == "open bracket") stack.Push(infixList[i]);
                    else if (TokenType(infixList[i]) == "close bracket")
                    {
                        if (stack.Count == 0)
                        {
                            Console.WriteLine($"Invalid Expression! Brackets must be used in appropriate pairs.");
                            return false;
                        }
                        string match = matchingParenthesis(infixList[i]);
                        if (stack.Pop() != match)
                        {
                            Console.WriteLine($"Invalid Expression! Mismatched brackets in your expression.");
                            return false;
                        }
                    }
                    else continue;
                }
                return true;
            }

            static void DisplayList(string message, List<string> list)
            {
                Console.WriteLine(message + " " + string.Join(" ", list));
            }
        }
    }
}
