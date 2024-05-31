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

        public void GenerarEstacion(int number, string shape, string station, bool isAvailable)
        {
            Console.SetCursorPosition(0, 0);

            int maxWidth = Math.Max(
                Math.Max($"Tipo de estación | {station}".Length, $"Disponibilidad   | {(isAvailable ? "Disponible" : "No disponible")}".Length),
                Math.Max($"Figura           | {shape}".Length, $"Queue  | {number}".Length)
            );

            string row = $"+-{new string('-', maxWidth)}-+";

            Console.WriteLine(row);
            Console.WriteLine($"| Tipo de estación | {station.PadRight(maxWidth - 19)} |");
            Console.WriteLine(row);
            Console.WriteLine($"| Disponibilidad   | {(isAvailable ? "Disponible" : "No disponible").PadRight(maxWidth - 19)} |");
            Console.WriteLine(row);
            Console.WriteLine($"| Figura           | {shape.PadRight(maxWidth - 19)} |");
            Console.WriteLine(row);

            switch (shape.ToLower())
            {
                case "cuadrado":
                    for (int i = 0; i < 5; i++)
                    {
                        Console.Write("| ");
                        Console.WriteLine(new string('*', 10).PadRight(maxWidth) + " |");
                    }
                    break;

                case "rectangulo":
                    for (int i = 0; i < 4; i++)
                    {
                        Console.Write("| ");
                        Console.WriteLine(new string('*', 14).PadRight(maxWidth) + " |");
                    }
                    break;

                case "circulo":
                    double radius = 5;
                    double thickness = 0.4;
                    double rIn = radius - thickness, rOut = radius + thickness;
                    for (double y = radius; y >= -radius; --y)
                    {
                        string line = "";
                        for (double x = -radius; x < rOut; x += 0.5)
                        {
                            double value = x * x + y * y;
                            if (value >= rIn * rIn && value <= rOut * rOut)
                            {
                                line += "*";
                            }
                            else
                            {
                                line += " ";
                            }
                        }
                        Console.Write("| " + line.PadRight(maxWidth) + " |");
                        Console.WriteLine();
                    }
                    break;

                case "triangulo":
                    int height = 5;
                    for (int i = 1; i <= height; i++)
                    {
                        Console.Write("| ");
                        string line = new string(' ', height - i) + new string('*', i * 2);
                        Console.WriteLine(line.PadRight(maxWidth) + " |");
                    }
                    break;

                case "vacio":
                    // No dibujar nada
                    break;

                default:
                    Console.WriteLine("| " + shape.PadRight(maxWidth) + " |");
                    break;
            }

            Console.WriteLine(row);
            Console.WriteLine($"| Queue            | {number.ToString().PadRight(maxWidth - 19)} |");
            Console.WriteLine(row);
        }


        public void MostrarProgreso(int milliseconds, int totalItems)
        {
            int totalTime = milliseconds;
            int interval = totalTime / totalItems;
            int progress = 0;

            Console.CursorVisible = false;

            while (totalTime > 0)
            {
                switch (progress % 11)
                {
                    case 0: Console.ForegroundColor = ConsoleColor.Red; break;
                    case 1: Console.ForegroundColor = ConsoleColor.Blue; break;
                    case 2: Console.ForegroundColor = ConsoleColor.Green; break;
                    case 3: Console.ForegroundColor = ConsoleColor.Cyan; break;
                    case 4: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case 5: Console.ForegroundColor = ConsoleColor.Magenta; break;
                    case 6: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                    case 7: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                    case 8: Console.ForegroundColor = ConsoleColor.Gray; break;
                    case 9: Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                    case 10: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                }
                progress++;
                Console.Write("[");
                Console.Write(new string('#', progress));
                Console.Write(new string(' ', totalItems - progress));
                Console.Write("]");

                Console.SetCursorPosition(Console.CursorLeft - (totalItems + 2), Console.CursorTop);
                Thread.Sleep(interval);
                totalTime -= interval;
            }

            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
