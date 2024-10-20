using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace QuadraticEquationSolver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("a * x^2 + b * x + c = 0");

            Console.WriteLine($"Введите значение a:");
            string a = Console.ReadLine();
            Console.WriteLine($"Введите значение b:");
            string b = Console.ReadLine();
            Console.WriteLine($"Введите значение c:");
            string c = Console.ReadLine();
            var parametersText = new Dictionary<string, string>() {
                { "a", a},
                { "b", b},
                { "c", c}
            };

            try
            {
                var parametersNumeric = ParseParameters(parametersText);
                Solver(parametersNumeric);
            }
            catch (OverflowException e)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine($"Значение {e.Data["value"]} не входит в диапазон допустимых значений.");
                Console.WriteLine($"Допустимые значения: от {int.MinValue} до {int.MaxValue}");
                Console.ResetColor();
            }
            catch (ParseException e)
            {
                FormatData(e.Message, Severity.Error, e.Data);
            }
            catch (SolverException e)
            {
                FormatData(e.Message, Severity.Warning, e.Data);
            }
        }

        enum Severity
        {
            Warning,
            Error
        }

        static void FormatData(string message, Severity severity, IDictionary data)
        {
            if (severity == Severity.Warning)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"{message}");

            var dataDictionary = data.Keys.Cast<object>().ToDictionary(k => k, k => data[k]);

            foreach (var item in dataDictionary)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }

            Console.WriteLine("--------------------------------------------------");

            Console.ResetColor();
        }

        static Dictionary<string, int> ParseParameters(Dictionary<string, string> parameters)
        {
            var numericParameters = new Dictionary<string, int>();
            foreach (var paramater in parameters)
            {
                var isNumber = BigInteger.TryParse(paramater.Value, out BigInteger value);
                if (isNumber == false)
                {
                    var ex = new ParseException($"Неверный формат параметра {paramater.Key}");
                    foreach (var item in parameters)
                    {
                        ex.Data.Add(item.Key, item.Value);
                    }
                    throw ex;
                }
                else
                {
                    var isInt = int.TryParse(paramater.Value, out int result);

                    if (isInt == false)
                    {
                        var ex = new OverflowException();
                        ex.Data.Add("value", value);
                        throw ex;
                    }
                    else
                    {
                        numericParameters.Add(paramater.Key, result);
                    }
                }
            }

            return numericParameters;
        }

        static void Solver(Dictionary<string, int> parameters)
        {
            int a = parameters["a"];
            int b = parameters["b"];
            int c = parameters["c"];

            double x1;
            double x2;
            double x;
            int d = b * b - 4 * a * c;

            if (d < 0)
            {
                var ex = new SolverException("Вещественных значений не найдено");
                foreach (var item in parameters)
                {
                    ex.Data.Add(item.Key, item.Value);
                }
                throw ex;
            }
            else if (d == 0)
            {
                x = (-b + Math.Sqrt(d)) / (2 * a);
                Console.WriteLine($"x = {x}");
            }
            else if (d > 0)
            {
                x1 = (-b + Math.Sqrt(d)) / (2 * a);
                x2 = (-b - Math.Sqrt(d)) / (2 * a);
                Console.WriteLine($"x1 = {x1}");
                Console.WriteLine($"x2 = {x2}");
            }
        }
    }

    class ParseException : FormatException
    {
        public ParseException(string message)
            : base(message)
        {
        }
    }
    class SolverException : Exception
    {
        public SolverException(string message)
            : base(message)
        {
        }
    }
}
