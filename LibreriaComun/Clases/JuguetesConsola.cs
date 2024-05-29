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

        static void GenerarEstacion(int number, string shape, string station, bool isAvailable)
        {
            int columnWidth = 24;

            Console.WriteLine("+-----------------------------+");
            Console.WriteLine("| Tipo de estación | " + station.PadRight(columnWidth - 16) + "|");
            Console.WriteLine("+-----------------------------+");
            Console.WriteLine("| Disponibilidad  | " + (isAvailable ? "Disponible" : "No disponible").PadRight(columnWidth - 16) + "|");
            Console.WriteLine("+-----------------------------+");
            Console.WriteLine("| Figura           | ");
            Console.WriteLine("+-----------------------------+");

            switch (shape.ToLower())
            {
                case "cuadrado":
                    for (int i = 0; i < 5; i++)
                    {
                        Console.Write("| ");
                        Console.WriteLine(new string('*', 5 * 2).PadRight(columnWidth - 1) + "|");
                    }
                    break;
                case "rectangulo":
                    for (int i = 0; i < 4; i++)
                    {
                        Console.Write("| ");
                        Console.WriteLine(new string('*', 7 * 2).PadRight(columnWidth - 1) + "|");
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
                        Console.Write("| " + line.PadRight(columnWidth - 1) + "|");
                        Console.WriteLine();
                    }
                    break;
                case "triangulo":
                    int height = 5;
                    for (int i = 1; i <= height; i++)
                    {
                        Console.Write("| ");
                        string line = new string(' ', height - i) + new string('*', i * 2);
                        Console.WriteLine(line.PadRight(columnWidth - 1) + "|");
                    }
                    break;
                case "vacio":
                    // No dibujar nada
                    break;
                default:
                    Console.WriteLine("| " + shape.PadRight(columnWidth - 1) + "|");
                    break;
            }

            Console.WriteLine("+-----------------------------+");
            Console.WriteLine("| Queue  | " + number.ToString().PadRight(columnWidth - 9) + "|");
            Console.WriteLine("+-----------------------------+");
        }

        public void MostrarProgreso(int milliseconds, int totalItems)
        {
            
            int totalTime = milliseconds; // ??????????
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
