using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreriaComun.Clases;
using LibreriaComun.DataContext;
using LibreriaComun.Modelos;
using Modelos = LibreriaComun.Modelos;


namespace MaquinaDeEstados
{
    class Program
    {
        static Modelos.DBLPFEntities db = DataContext.ObtenerInstancia();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Menu();
            Console.ReadKey();
        }

        public void Menu()  //Crear el menu de estaciones con la base de datos
        {
            while (true)
            {
                List<Modelos.Estacion_Trabajo> Estaciones = db.Estacion_Trabajo.ToList();

                foreach (var item in Estaciones)
                {
                    Console.WriteLine(item.ID + "- Estacion: " + item.Nombre);
                }

                int estacion = int.Parse(Console.ReadLine());

                if (Estaciones.Any(x => x.ID == estacion))
                {
                    Estacion(estacion);
                }
                else
                {
                    Console.WriteLine("Wtf pala");
                }
            }
        }

        public void Estacion(int id_estacion)
        {
            //Define cuando salir del bucle para esta estacion
            bool salir = false;

            while (!salir)
            {
                //Ver si está ocupada la estacion
                Modelos.EstacionDyM EstacionDyT = ItemController.ObtenerDisponibilidadYModo(id_estacion);

                //Listar los items que están en espera
                List<Item> items = ItemController.ObtenerFilaDeEstacion(id_estacion);

                Console.Clear();

                //imprime la estacion
                JuguetesConsola consola = new JuguetesConsola();
                consola.GirarBarrita(1000);
                consola.GenerarEstacion(items.Count,EstacionDyT.Figura,EstacionDyT.Estacion,EstacionDyT.Disponibilidad);

                Console.WriteLine("1.- Avanzar");
                Console.WriteLine("2.- Siguiente");
                Console.WriteLine("3.- Descartar");
                Console.WriteLine("4.- Cambiar Modo");
                Console.WriteLine("5.- Salir");
                Console.WriteLine("6.- Actualizar");

                int opcion = int.Parse(Console.ReadLine());
                
                switch (opcion)
                {
                    case 1:
                        if (ItemController.AvanzarFigura(id_estacion))
                        {
                            consola.GirarBarrita(2000);
                            Console.WriteLine("Exito crack, ha avanzado la figura sin problema");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("No se encuentra ninguna figura en la estacion Pala nmms estas ciego?");
                        };
                        Console.ReadKey();
                        continue;

                    case 2:
                        if(ItemController.Siguiente(id_estacion, items))
                        {
                            consola.GirarBarrita(1000);
                            Console.WriteLine("Exito crack, ha pasado la siguiente figura sin problema");
                        }
                        Console.ReadKey();
                        continue;

                    case 3:

                        if (ItemController.Scrap(id_estacion))
                        {
                            consola.GirarBarrita(2000);
                            Console.WriteLine("Exito crack, haz descartado la figura");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("No se encuentra ninguna figura en la estacion Pala nmms estas ciego?");
                        };
                        Console.ReadKey();
                        continue;

                    case 4:
                        Console.Clear();
                        consola.GirarBarrita(1000);
                        //CambiarModo(id_estacion);
                        break;

                    case 5:
                        Console.Clear();
                        consola.GirarBarrita(1000);
                        //Salir();
                        salir = true;
                        return;

                    case 6:
                        consola.GirarBarrita(500);
                        continue;

                    default:
                        break;
                }
            }
            
        }

       

        private static void StartItem(int id)
        {
            Task.Run(() =>
            {
                //Cuenta la cantidad de estados y se va actualiando dependiendo del item


                //Obtener el modelo del item recibido
                Item item = db.Item.FirstOrDefault(x => x.ID == id);

                //detectar el Estado que sigue
                Evento Evento = item.Estado.Evento.Where(x => x.Estado_Final != 99).FirstOrDefault();
                if (Evento == null)
                {
                    return; //Se termina el ciclo de la figura y destruye el hilo
                }
                //El estado que sigue tiene estacion o está en espera
                int EstadoSiguiente = Evento.Estado_Final;
                //
                int EstacionActual = item.Estado.ID_Estacion.Value;
                //
                int EstacionSiguiente = (int)db.Estado.Where(x => x.ID == EstadoSiguiente).Select(x => x.ID_Estacion).FirstOrDefault();


            //condicional, que si la estacion que sigue es 0 estará en espera, si es 1 u otro pasa a verificacion de disponibilidad
            if (EstacionSiguiente != 0)
            {
                    //ve si hay alguien adelante de el (Que esté en el mismo estado que tu pero adelante de ti)
                    List<Historico> fila = db.Historico.Where(x => x.ID_Evento == Evento.ID).ToList(); //Lista de todos las figuras que estan actualmemte en ese estado

                    int posicion = fila.FindIndex(x => x.ID_Evento == Evento.ID);

                    if (posicion != -1 && posicion != 0) //significa que hay mas en espera, ahora verificar si es el siguiente en entrar
                    {
                        return; //regresa hasta que este desocupado
                    }
                    
                    //Verifica si hay disponibilidad de la estación y el modo
                    Modelos.Estacion_Trabajo Estacion = db.Estacion_Trabajo.Where(x => x.ID == EstacionSiguiente).FirstOrDefault();

                    //Si está disponible y el modo corresponde a la de la figura entra al nuevo estado y se modifica la estacion a ocupado
                    if (Estacion.ID_Estado_Trabajo == 1 && Estacion.Modo_ID_Figura == item.ID_Figura)
                    {
                        try
                        {
                            Estacion.ID_Estado_Trabajo = 2; //poner ocupado la estacion
                            item.ID_Estado = EstadoSiguiente; //coloco en nuevo estado en el item

                            //registro que del historico tanto del item como de la estacion
                            Modelos.Historico historicoItem = new Modelos.Historico
                            {
                                ID_Item = item.ID,
                                ID_Evento = Evento.ID,
                                IsActive = true,
                                Edit_Date = DateTime.UtcNow,
                                Origin_Date = DateTime.UtcNow,
                                Tiempo = DateTime.UtcNow.TimeOfDay
                            };

                            Modelos.Historico_Estacion_Trabajo historicoEstacion = new Historico_Estacion_Trabajo
                            {
                                ID_Estacion_Trabajo = Estacion.ID,
                                ID_Estado_Estacion_Trabajo = Estacion.ID_Estado_Trabajo,
                                ID_Modo = Estacion.Modo_ID_Figura,
                                IsActive = true,
                                Origin_Date = DateTime.UtcNow,
                                Edit_Date = DateTime.UtcNow,
                                Tiempo_Estacion_Trabajo = DateTime.UtcNow.TimeOfDay
                            };

                            db.Historico.Add(historicoItem);
                            db.Historico_Estacion_Trabajo.Add(historicoEstacion);

                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else    //Si no está disponible se repite el proceso hasta que esté disponible
                    {
                        return;
                    }
                }
                else
                {
                    Thread.Sleep(10000);
                    {
                        Modelos.Estacion_Trabajo Estacion = db.Estacion_Trabajo.Where(x => x.ID == EstacionActual).FirstOrDefault();

                        try     //colocamos el item en el siguiente estado de espera
                        {
                            item.ID_Estado = EstadoSiguiente;

                            Modelos.Historico historicoItem = new Modelos.Historico
                            {
                                ID_Item = item.ID,
                                ID_Evento = Evento.ID,
                                IsActive = true,
                                Edit_Date = DateTime.UtcNow,
                                Origin_Date = DateTime.UtcNow,
                                Tiempo = DateTime.UtcNow.TimeOfDay

                            };

                            Estacion.ID_Estado_Trabajo = 1; //poner en disponible la estacion y registrar su historial
                            Modelos.Historico_Estacion_Trabajo historicoEstacion = new Historico_Estacion_Trabajo
                            {
                                ID_Estacion_Trabajo = Estacion.ID,
                                ID_Estado_Estacion_Trabajo = Estacion.ID_Estado_Trabajo,
                                ID_Modo = Estacion.Modo_ID_Figura,
                                IsActive = true,
                                Origin_Date = DateTime.UtcNow,
                                Edit_Date = DateTime.UtcNow,
                                Tiempo_Estacion_Trabajo = DateTime.UtcNow.TimeOfDay
                            };

                            db.Historico_Estacion_Trabajo.Add(historicoEstacion);

                            db.Historico.Add(historicoItem);

                            db.SaveChanges();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }//Validacion de si es el ultimo estado de todos
                    }
                }
            });
        }
    }
}