using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreriaComun.DataContext;
using LibreriaComun.Clases;
using static LineaFigurasV2.Clases.ColoresWindows;
using Modelos = LibreriaComun.Modelos;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LineaFigurasV2
{
    class Program
    {
        static Modelos.DBLPFEntities db = DataContext.ObtenerInstancia();

        static async Task Main(string[] args)
        {

            JuguetesConsola JuguetesConsola = new JuguetesConsola();

            // Obtiene el catalogo de datos, che fer
            List<Modelos.Figura> figuras = db.Figura.ToList();
            List<Modelos.Color> colores = db.Color.ToList();

            while (true)
            {
                Console.WriteLine("Seleccione el tipo de figura a crear:");

                // Mostrar opciones de figuras
                foreach (Modelos.Figura f in figuras)
                {
                    Console.WriteLine($"|{f.ID}| {f.Figura1}");
                }

                int opcionFigura = Convert.ToInt32(Console.ReadLine());
                if (!figuras.Any(x => x.ID == opcionFigura))
                {
                    Console.WriteLine("Opción inexistente");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                Console.WriteLine("Ingrese el color:");

                // Mostrar opciones de colores
                foreach (Modelos.Color c in colores)
                {
                    Console.WriteLine($"|{c.ID}| {c.Color1}");
                }

                int opcionColor = Convert.ToInt32(Console.ReadLine());

                if (!colores.Any(x => x.ID == opcionColor))
                {
                    Console.WriteLine("Opción inexistente");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                // Asigno en db los valores default e ingresados por el usuario
                var nuevoItem = new Modelos.Item
                {
                    ID_Figura = opcionFigura,
                    ID_Color = opcionColor,
                    ID_Estado = 0
                };

                db.Item.Add(nuevoItem);
                db.SaveChanges();

                // Te instancio el receptor con el ID del nuevo item

                // Instanciame esta
                int nuevoItemId = nuevoItem.ID;

                await NamedPipeHelper.EnviarVariableAsync(nuevoItemId);

                Console.WriteLine($"Item creado con ID: {nuevoItemId}");
                Console.ReadKey();
                Console.Clear();
            }
        }


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
        }
    }

}
