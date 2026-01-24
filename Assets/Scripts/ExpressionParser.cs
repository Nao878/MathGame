using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Expression parser that evaluates mathematical expressions for a given x value.
/// Supports: +, -, *, /, ^ (power), parentheses, variable x
/// </summary>
public class ExpressionParser
{
    private string expression;
    private int pos;
    private double xValue;

    /// <summary>
    /// Parse and evaluate a mathematical expression string for a given x value.
    /// </summary>
    /// <param name="input">Expression string (e.g., "x", "2*x + 1", "x^2")</param>
    /// <param name="x">Value to substitute for x</param>
    /// <returns>Result of the calculation, or null if error</returns>
    public static double? Evaluate(string input, double x)
    {
        try
        {
            // Remove "y = " prefix
            string expr = input.Trim();
            if (expr.StartsWith("y=", StringComparison.OrdinalIgnoreCase))
            {
                expr = expr.Substring(2).Trim();
            }
            else if (expr.StartsWith("y =", StringComparison.OrdinalIgnoreCase))
            {
                expr = expr.Substring(3).Trim();
            }

            // Remove whitespace
            expr = expr.Replace(" ", "");

            // Add implicit multiplication (e.g., 2x -> 2*x, x(2) -> x*(2))
            expr = AddImplicitMultiplication(expr);

            var parser = new ExpressionParser(expr, x);
            return parser.Parse();
        }
        catch (Exception e)
        {
            Debug.LogError($"Expression parse error: {e.Message} - Input: {input}");
            return null;
        }
    }

    private static string AddImplicitMultiplication(string expr)
    {
        // Insert * between digit and x (e.g., 2x -> 2*x)
        expr = Regex.Replace(expr, @"(\d)([x(])", "$1*$2");
        // Insert * between x and digit (e.g., x2 -> x*2)
        expr = Regex.Replace(expr, @"([x)])(\d)", "$1*$2");
        // Insert * between x and parenthesis (e.g., x(2) -> x*(2))
        expr = Regex.Replace(expr, @"([x)])([(])", "$1*$2");
        // Insert * between parenthesis and x (e.g., (2)x -> (2)*x)
        expr = Regex.Replace(expr, @"([)])([x])", "$1*$2");
        return expr;
    }

    private ExpressionParser(string expression, double x)
    {
        this.expression = expression;
        this.xValue = x;
        this.pos = 0;
    }

    private double Parse()
    {
        double result = ParseAddSub();
        if (pos < expression.Length)
        {
            throw new Exception($"Unexpected character: {expression[pos]}");
        }
        return result;
    }

    // Addition and subtraction
    private double ParseAddSub()
    {
        double left = ParseMulDiv();

        while (pos < expression.Length)
        {
            char op = expression[pos];
            if (op == '+')
            {
                pos++;
                left += ParseMulDiv();
            }
            else if (op == '-')
            {
                pos++;
                left -= ParseMulDiv();
            }
            else
            {
                break;
            }
        }

        return left;
    }

    // Multiplication and division
    private double ParseMulDiv()
    {
        double left = ParsePower();

        while (pos < expression.Length)
        {
            char op = expression[pos];
            if (op == '*')
            {
                pos++;
                left *= ParsePower();
            }
            else if (op == '/')
            {
                pos++;
                double right = ParsePower();
                if (right == 0)
                {
                    throw new Exception("Division by zero error");
                }
                left /= right;
            }
            else
            {
                break;
            }
        }

        return left;
    }

    // Power (right associative)
    private double ParsePower()
    {
        double baseValue = ParseUnary();

        if (pos < expression.Length && expression[pos] == '^')
        {
            pos++;
            double exponent = ParsePower(); // Right associative recursion
            return Math.Pow(baseValue, exponent);
        }

        return baseValue;
    }

    // Unary operators (sign)
    private double ParseUnary()
    {
        if (pos < expression.Length)
        {
            if (expression[pos] == '+')
            {
                pos++;
                return ParseUnary();
            }
            else if (expression[pos] == '-')
            {
                pos++;
                return -ParseUnary();
            }
        }

        return ParsePrimary();
    }

    // Primary elements (numbers, variable x, parentheses)
    private double ParsePrimary()
    {
        // Parentheses
        if (pos < expression.Length && expression[pos] == '(')
        {
            pos++; // Skip '('
            double result = ParseAddSub();
            if (pos >= expression.Length || expression[pos] != ')')
            {
                throw new Exception("Missing closing parenthesis");
            }
            pos++; // Skip ')'
            return result;
        }

        // Variable x
        if (pos < expression.Length && expression[pos] == 'x')
        {
            pos++;
            return xValue;
        }

        // Number
        int startPos = pos;
        while (pos < expression.Length && (char.IsDigit(expression[pos]) || expression[pos] == '.'))
        {
            pos++;
        }

        if (startPos == pos)
        {
            throw new Exception($"Number or variable expected at position: {pos}");
        }

        string numStr = expression.Substring(startPos, pos - startPos);
        if (double.TryParse(numStr, out double value))
        {
            return value;
        }
        else
        {
            throw new Exception($"Invalid number: {numStr}");
        }
    }
}
