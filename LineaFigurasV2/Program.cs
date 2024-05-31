﻿using System;
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
using LibreriaComun.Modelos;
using System.Runtime.Remoting.Contexts;

namespace LineaFigurasV2
{
    class Program
    {
        static Modelos.DBLPFEntities db = DataContext.ObtenerInstancia();

        static async Task Main(string[] args)
        {
            JuguetesConsola JuguetesConsola = new JuguetesConsola();

            // Obtiene el catálogo de datos
            List<Modelos.Figura> figuras = db.Figura.ToList();
            List<Modelos.Color> colores = db.Color.ToList();

            while (true)
            {
                Console.WriteLine("------- Seleccionar Modo ------");
                Console.WriteLine("----- 1. Cancelar Figuras -----");
                Console.WriteLine("----- 2. Crear una Figura -----");
                Console.WriteLine("----- 3. Salir ----------------");
                string opcionuser = Console.ReadLine();
                int.TryParse(opcionuser, out int opcionModo);

                Console.Clear();

                switch (opcionModo)
                {
                    case 1:
                        CancelarFiguras();
                        break;
                    case 2:
                        CrearFigura(figuras, colores);
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Opción no válida.");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void CancelarFiguras()
        {
            List<Item> items = ItemController.ObtenerFilaDeEstacion(1);
            ItemController.ListaFiguras(items);
            int idEspecifico = ItemController.InputVerifier(items);

            Item item = items[idEspecifico];

            if (item != null)
            {
                ItemController.Cancelar(item);
                db.SaveChanges(); // Asegúrate de guardar los cambios en la base de datos
                Console.WriteLine($"El item con ID {item.ID} ha sido cancelado.");
            }
            else
            {
                Console.WriteLine("Item no encontrado.");
            }
            Console.ReadKey();
            Console.Clear();
        }

        static void CrearFigura(List<Modelos.Figura> figuras, List<Modelos.Color> colores)
        {
            Console.WriteLine("Seleccione el tipo de figura a crear:");
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
                return;
            }

            Console.WriteLine("Ingrese el color:");
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
                return;
            }

            var nuevoItem = new Modelos.Item
            {
                ID_Figura = opcionFigura,
                ID_Color = opcionColor,
                ID_Estado = 0,
                IsActive = true,
                Origin_Date = DateTime.UtcNow,
                Edit_Date = DateTime.UtcNow
            };

            db.Item.Add(nuevoItem);
            db.SaveChanges();

            int nuevoItemId = nuevoItem.ID;
            Console.WriteLine($"Item creado con ID: {nuevoItemId}");
            Console.ReadKey();
            Console.Clear();
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