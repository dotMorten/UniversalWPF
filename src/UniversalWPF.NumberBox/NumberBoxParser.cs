using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace UniversalWPF
{
    enum MathTokenType
    {
        Numeric,
        Operator,
        Parenthesis,
    }
    struct MathToken
    {
        public MathTokenType Type;
        public char Char;
        public double Value;

        public MathToken(MathTokenType type, char ch) : this()
        {
            this.Type = type;
            Char = ch;
        }
        public MathToken(MathTokenType type, double value) : this()
        {
            this.Type = type;
            Value = value;
        }
    }
    internal static class NumberBoxParser
    {
        const string c_numberBoxOperators = "+-*/^";


        private static List<MathToken> GetTokens(string input, IFormatProvider numberParser)
        {
            var tokens = new List<MathToken>();

            bool expectNumber = true;
            int i = 0;
            while (i < input.Length)
            {
                // Skip spaces
                var nextChar = input[i];
                if (nextChar != ' ')
                {
                    if (expectNumber)
                    {
                        if (nextChar == '(')
                        {
                            // Open parens are also acceptable, but don't change the next expected token type.
                            tokens.Add(new MathToken(MathTokenType.Parenthesis, nextChar));
                        }
                        else
                        {
                            var result  = GetNextNumber(input.Substring(i), numberParser);
                            var value = result.Item1;
                            var charLength = result.Item2;

                            if (charLength > 0)
                            {
                                tokens.Add(new MathToken(MathTokenType.Numeric, value));
                                i += charLength - 1; // advance the end of the token
                                expectNumber = false; // next token should be an operator
                            }
                            else
                            {
                                // Error case -- next token is not a number
                                return new List<MathToken>();
                            }
                        }
                    }
                    else
                    {
                        if (c_numberBoxOperators.IndexOf(nextChar) > -1)
                        {
                            tokens.Add(new MathToken(MathTokenType.Operator, nextChar));
                            expectNumber = true; // next token should be a number
                        }
                        else if (nextChar == ')')
                        {
                            // Closed parens are also acceptable, but don't change the next expected token type.
                            tokens.Add(new MathToken(MathTokenType.Parenthesis, nextChar));
                        }
                        else
                        {
                            // Error case -- could not evaluate part of the expression
                            return new List<MathToken>();
                        }
                    }
                }

                i++;
            }

            return tokens;
        }

        // Attempts to parse a number from the beginning of the given input string. Returns the character size of the matched string.
        private static Tuple<double, int> GetNextNumber(string input, IFormatProvider numberParser)
        {
            // Attempt to parse anything before an operator or space as a number
            string regex = "^-?([^-+/*\\(\\)\\^\\s]+)";
            Match match = Regex.Match(input, regex);
            if (match.Success)
            {
                // Might be a number
                if (double.TryParse(match.Value, NumberStyles.Any, numberParser, out double parsedNum))
                {
                    // Parsing was successful
                    return new Tuple<double, int>(parsedNum, match.Length);
                }
            }

            return new Tuple<double, int>(double.NaN, 0);
        }

        private static int GetPrecedenceValue(char c)
        {
            int opPrecedence = 0;
            if (c == '*' || c == '/')
            {
                opPrecedence = 1;
            }
            else if (c == '^')
            {
                opPrecedence = 2;
            }

            return opPrecedence;
        }

        // Converts a list of tokens from infix format (e.g. "3 + 5") to postfix (e.g. "3 5 +")
        private static List<MathToken> ConvertInfixToPostfix(List<MathToken> infixTokens)
        {
            List<MathToken> postfixTokens = new List<MathToken>();
            Stack<MathToken> operatorStack = new Stack<MathToken>();

            foreach (var token in infixTokens)
            {
                if (token.Type == MathTokenType.Numeric)
                {
                    postfixTokens.Add(token);
                }
                else if (token.Type == MathTokenType.Operator)
                {
                    while (operatorStack.Count > 0)
                    {
                        var top = operatorStack.Peek();
                        if (top.Type != MathTokenType.Parenthesis && (GetPrecedenceValue(top.Char) >= GetPrecedenceValue(token.Char)))
                        {
                            postfixTokens.Add(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                    operatorStack.Push(token);
                }
                else if (token.Type == MathTokenType.Parenthesis)
                {
                    if (token.Char == '(')
                    {
                        operatorStack.Push(token);
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek().Char != '(')
                        {
                            // Pop operators onto output until we reach a left paren
                            postfixTokens.Add(operatorStack.Pop());
                        }

                        if (operatorStack.Count == 0)
                        {
                            // Broken parenthesis
                            return new List<MathToken>();
                        }

                        // Pop left paren and discard
                        operatorStack.Pop();
                    }
                }
            }

            // Pop all remaining operators.
            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek().Type == MathTokenType.Parenthesis)
                {
                    // Broken parenthesis
                    return new List<MathToken>();
                }

                postfixTokens.Add(operatorStack.Pop());
            }

            return postfixTokens;
        }

        private static double? ComputePostfixExpression(List<MathToken> tokens)
        {
            Stack<double> stack = new Stack<double>();

            foreach (var token in tokens)
            {
                if (token.Type == MathTokenType.Operator)
                {
                    // There has to be at least two values on the stack to apply
                    if (stack.Count < 2)
                    {
                        return null;
                    }

                    var op1 = stack.Pop();
                    var op2 = stack.Pop();

                    double result;

                    switch (token.Char)
                    {
                        case '-':
                            result = op2 - op1;
                            break;

                        case '+':
                            result = op1 + op2;
                            break;

                        case '*':
                            result = op1 * op2;
                            break;

                        case '/':
                            if (op1 == 0)
                            {
                                // divide by zero
                                return double.NaN;
                            }
                            else
                            {
                                result = op2 / op1;
                            }
                            break;

                        case '^':
                            result = Math.Pow(op2, op1);
                            break;

                        default:
                            return null;
                    }

                    stack.Push(result);
                }
                else if (token.Type == MathTokenType.Numeric)
                {
                    stack.Push(token.Value);
                }
            }

            // If there is more than one number on the stack, we didn't have enough operations, which is also an error.
            if (stack.Count != 1)
            {
                return null;
            }

            return stack.Peek();
        }

        public static double? Compute(string expr, IFormatProvider numberParser)
        {
            // Tokenize the input string
            var tokens = GetTokens(expr, numberParser);
            if (tokens.Count > 0)
            {
                // Rearrange to postfix notation
                var postfixTokens = ConvertInfixToPostfix(tokens);
                if (postfixTokens.Count > 0)
                {
                    // Compute expression
                    return ComputePostfixExpression(postfixTokens);
                }
            }

            return null;
        }
    }
}