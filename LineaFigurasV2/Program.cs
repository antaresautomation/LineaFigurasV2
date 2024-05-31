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

            // Obtiene el catalogo de datos, che fer
            List<Modelos.Figura> figuras = db.Figura.ToList();
            List<Modelos.Color> colores = db.Color.ToList();

            while (true)
            {
                Console.WriteLine("----- Seleccionar Modo ------");
                Console.WriteLine("----- 1. Cancelar Figuras -----");
                Console.WriteLine("----- 2. Crear una Figura -----");
                Console.WriteLine("----- 3. Salir ----------------");
                string opcionuser = Console.ReadLine();
                int.TryParse(opcionuser, out int opcionModo);

                if (opcionModo == 3)
                {
                    break; // Salir del while
                }

                Console.Clear();

                switch (opcionModo)
                {
                    case 1:
                        List<Item> items = ItemController.ObtenerFilaDeEstacion(1);
                        ItemController.ListaFiguras(items);
                        int idEspecifico = ItemController.InputVerifier(items);
                        int codigoParaSalir = -2;
                        if (idEspecifico == codigoParaSalir || idEspecifico == -1)
                        {
                            Console.Clear();
                            break;
                        }

                        Item item = items[idEspecifico];
                        //db.Item.SingleOrDefault(i => i.ID == idEspecifico);

                        if (item != null)
                        {
                            ItemController.Cancelar(item);
                            db.SaveChanges(); 
                            JuguetesConsola.GirarBarrita(3000);
                            Console.WriteLine($"El item con ID {item.ID} ha sido cancelado.");
                        }
                        else
                        {
                            JuguetesConsola.GirarBarrita(1000);
                            Console.WriteLine("Item no encontrado.");
                        }
                        break;

                    case 2:
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
                            continue;
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
                            continue;
                        }

                        // Asigno en db los valores default e ingresados por el usuario
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

                        // Te instancio el receptor con el ID del nuevo item

                        // Instanciame esta
                        int nuevoItemId = nuevoItem.ID;

                        Console.WriteLine($"Item creado con ID: {nuevoItemId}");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    default:
                        Console.WriteLine("No estuviste el dia que repartieron ojos?");
                        Thread.Sleep(2000);
                        Console.WriteLine("Aca van 10 segundos de castigo...");
                        Thread.Sleep(2000);
                        JuguetesConsola.GirarBarrita(10000);
                        Console.Clear();
                        break;
                }
            }
        }
    }
}
