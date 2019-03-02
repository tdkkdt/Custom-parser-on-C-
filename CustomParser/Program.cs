using System;
using System.Collections.Generic;

namespace CustomParser {
    class Program {
        static void Main(string[] args) {
            RunTests();
        }

        static void RunTests() {
            Check("1 1 +", 2d);
            Check("2 -3 *", -6d);
            Check("1 1 + 2 3 * -", -4d);
            Check("1 3 / 2 *", 0.666666666666667d);
            Check("10 1 2 + 2 + 5 + +", 20d);
        }

        static void Check(string expr, double expectedResult) {
            var actualResult = ParseAndCalc(expr);
            if (Math.Abs(actualResult - expectedResult) > 1e-14) {
                throw new Exception($"Expected {expectedResult} but {actualResult} found");
            }
        }

        static ReadOnlySpan<char> GetNextToken(ReadOnlySpan<char> exprSpan, ref int i) {
            while (i < exprSpan.Length && char.IsWhiteSpace(exprSpan[i])) {
                i++;
            }
            int l = i;
            while (i < exprSpan.Length && !char.IsWhiteSpace(exprSpan[i])) {
                i++;
            }
            return exprSpan.Slice(l, i - l);
        }

        static bool TryToParseOperation(ReadOnlySpan<char> token, out Operation operation) {
            operation = default;
            if (token.Length != 1) {
                return false;
            }
            switch (token[0]) {
                case '+': {
                    operation = Operation.Add;
                    return true;
                }
                case '-': {
                    operation = Operation.Sub;
                    return true;
                }
                case '*': {
                    operation = Operation.Mul;
                    return true;
                }
                case '/': {
                    operation = Operation.Div;
                    return true;
                }
            }
            return false;
        }

        static double ParseAndCalc(string expr) {
            var exprSpan = expr.AsSpan();
            int i = 0;
            var stack = new Stack<double>();
            while (i < exprSpan.Length) {
                var token = GetNextToken(exprSpan, ref i);
                if (TryToParseOperation(token, out Operation operation)) {
                    var b = stack.Pop();
                    var a = stack.Pop();
                    double c;
                    switch (operation) {
                        case Operation.Add:
                            c = a + b;
                            break;
                        case Operation.Sub:
                            c = a - b;
                            break;
                        case Operation.Mul:
                            c = a * b;
                            break;
                        case Operation.Div:
                            c = a / b;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    stack.Push(c);
                }
                else {
                    stack.Push(Int32.Parse(token));
                }
            }
            if (stack.Count != 1) {
                throw  new Exception();
            }
            return stack.Pop();
        }
    }

    enum Operation {
        Add,
        Sub,
        Mul,
        Div,
    }
}
