using System.Collections;
using System.Numerics;

namespace QuadraticEquationSolver
{
    internal class Program
    {
        private static readonly string[] labels = { "a", "b", "c" };

        static void Main(string[] args)
        {
            string[] inputs = new string[3] { "", "", "" };
            int selected = 0;

            ConsoleKey key;
            do
            {
                Console.Clear();

                Console.WriteLine($"{FormatEquation(inputs)}");

                for (int i = 0; i < 3; i++)
                    Console.WriteLine($"{(i == selected ? ">" : " ")} {labels[i]}: {inputs[i]}");

                var keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selected = Math.Max(0, selected - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selected = Math.Min(2, selected + 1);
                        break;
                    case ConsoleKey.Backspace:
                        if (inputs[selected].Length > 0)
                            inputs[selected] = inputs[selected][..^1];
                        break;
                    case ConsoleKey.Enter:
                        break;
                    default:
                        inputs[selected] += keyInfo.KeyChar;
                        break;
                }
            } while (key != ConsoleKey.Enter);

            var parametersText = new Dictionary<string, string>() {
                { labels[0], inputs[0] },
                { labels[1], inputs[1] },
                { labels[2], inputs[2] }
            };
            try
            {
                var parametersNumeric = ParseParameters(parametersText);

                Solve(parametersNumeric);
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

        static string FormatEquation(string[] inputs)
        {
            string GetValue(int index) =>
                int.TryParse(inputs[index], out _) ? inputs[index] : labels[index].ToString();

            return $"{GetValue(0)}x^2 + {GetValue(1)}x + {GetValue(2)} = 0";
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

        static void Solve(Dictionary<string, int> parameters)
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
}