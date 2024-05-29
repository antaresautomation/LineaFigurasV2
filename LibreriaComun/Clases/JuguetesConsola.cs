using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibreriaComun.Clases
{
    public class JuguetesConsola
    {
        int counter;
        public JuguetesConsola()
        {
            counter = 0;
        }
        public void GirarBarrita(int milliseconds)
        {
            int totalTime = milliseconds;
            int interval = 100;

            Console.CursorVisible = false;

            while (totalTime > 0)
            {
                switch (counter % 4)
                {
                    case 0: Console.Write("/"); Console.ForegroundColor = ConsoleColor.Red; break;
                    case 1: Console.Write("-"); Console.ForegroundColor = ConsoleColor.Blue; break;
                    case 2: Console.Write("\\"); Console.ForegroundColor = ConsoleColor.Green; break;
                    case 3: Console.Write("|"); Console.ForegroundColor = ConsoleColor.Cyan; break;
                }
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Thread.Sleep(interval);
                totalTime -= interval;
                counter++;
            }

            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void GenerarEstacion(int number, string shape, string station)
        {
            Console.WriteLine("+---------------------------------+");
            Console.WriteLine("| Queue  | " + station.PadRight(23) + "|");
            Console.WriteLine("+---------------------------------+");
            Console.WriteLine($"| {number.ToString().PadRight(6)} | {shape.PadRight(22)} |");
            Console.WriteLine("+---------------------------------+");

            switch (shape.ToLower())
            {
                case "cuadrado":
                    for (int i = 0; i < 5; i++)
                    {
                        Console.Write("|        | ");
                        Console.WriteLine(new string('*', 5 * 2));
                    }
                    break;
                case "rectangulo":
                    for (int i = 0; i < 4; i++)
                    {
                        Console.Write("|        | ");
                        Console.WriteLine(new string('*', 7 * 2));
                    }
                    break;
                case "circulo":
                    double radius = 5;
                    double thickness = 0.4;
                    double rIn = radius - thickness, rOut = radius + thickness;
                    for (double y = radius; y >= -radius; --y)
                    {
                        Console.Write("|        | ");
                        for (double x = -radius; x < rOut; x += 0.5)
                        {
                            double value = x * x + y * y;
                            if (value >= rIn * rIn && value <= rOut * rOut)
                            {
                                Console.Write("*");
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        Console.WriteLine();
                    }
                    break;
                case "triangulo":
                    int height = 5;
                    for (int i = 1; i <= height; i++)
                    {
                        Console.Write("|        | ");
                        Console.WriteLine(new string(' ', height - i) + new string('*', i * 2));
                    }
                    break;
                case "vacio":
                    // No dibujar nada
                    break;
                default:
                    Console.WriteLine("|        | ocupada       |");
                    break;
            }

            Console.WriteLine("+---------------------------------+");
        }
    }
}
