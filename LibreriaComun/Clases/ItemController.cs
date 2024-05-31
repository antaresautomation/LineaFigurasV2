using LibreriaComun.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LibreriaComun.Clases
{
    public class ItemController
    {
        static DBLPFEntities db = DataContext.DataContext.ObtenerInstancia();
        // Retorna el estado siguiente de un item
        public static Evento ObtenerEvento(Item item)
        {
            Evento evento = item.Estado.Evento.Where(x => x.Estado_Final != 99).FirstOrDefault();
            return evento;
        }
        public static Evento ObtenerEventoScrap(Item item)
        {
            Evento evento = item.Estado.Evento.Where(x => x.Estado_Final == 99).FirstOrDefault();
            return evento;
        }
        public static int ObtenerEstadoSiguiente(Item item)
        {
            Evento evento = ObtenerEvento(item);
            return evento != null ? evento.Estado_Final : -1;
        }
        public static Estacion_Trabajo ObtenerEstacion(int ID_Estacion_Trabajo)
        {
            Estacion_Trabajo estacionDyT =db.Estacion_Trabajo.Where(x => x.ID == ID_Estacion_Trabajo).FirstOrDefault();
            return estacionDyT;
        }

        // Verifica disponibilidad de una estacion
        public static bool VerificarModo(int estacionSiguiente, int idFigura)
        {

            Estacion_Trabajo estacion = ObtenerEstacion(estacionSiguiente);
            return estacion != null && estacion.Modo_ID_Figura == idFigura;
        }
        public static bool VerificarDisponibilidad(int estacionSiguiente, int idFigura)
        {
            
            Estacion_Trabajo estacion = ObtenerEstacion(estacionSiguiente);
            return estacion != null && estacion.ID_Estado_Trabajo == 1;
        }

        //Obtiene ka Disponibilidad y el modo de la estacion
        public static Modelos.EstacionDyM ObtenerDisponibilidadYModo(int estacion)
        {
            Estacion_Trabajo estacionDyT = ObtenerEstacion(estacion);
            EstacionDyM DyT = new EstacionDyM
            {
                Disponibilidad = estacionDyT.Estado_Estacion_Trabajo.Disponible,
                Modo = estacionDyT.Modo_ID_Figura,
                Estacion = estacionDyT.Nombre,
                Figura = estacionDyT.Figura.Figura1
            };
            return DyT;
        }

        //Obtiene la lista que estan en fila antes de entrar a la estacion
        public static List<Item> ObtenerFilaDeEstacion(int EstacionID)
        {

            //consigo el id del estado a que corresponde
            int id_estado = db.Estado.Where(x => x.ID_Estacion == EstacionID).Select(x => x.ID).FirstOrDefault();
            //Consigo el Estado inicial para buscar los items
            int EstadoAnterior = db.Evento.Where(x => x.Estado_Final == id_estado).Select(x => x.Estado_Inicial).FirstOrDefault();
            //Ahora busco los items que estan en espera
            List<Item> fila = db.Item.Where(x => x.ID_Estado == EstadoAnterior).ToList();

            return fila;
        }

        //Obtiene el Item que está actualmente en la estacion proporcionada

        public static Item ObtenerItemEstacion(int EstacionID)
        {
            int EstadoEstacion = db.Estado.Where(x => x.ID_Estacion == EstacionID).Select(x => x.ID).FirstOrDefault();

            Item item = db.Item.Where(x => x.ID_Estado == EstadoEstacion).FirstOrDefault();

            return item;
        }

        // El nombre de la funcion explica lo que hace, no se que escribir aca
        public static void RegistrarHistoricoItem(Item item, Evento evento)
        {
            Modelos.Historico historicoItem = new Modelos.Historico
            {
                ID_Item = item.ID,
                ID_Evento = evento.ID,
                IsActive = true,
                Edit_Date = DateTime.UtcNow,
                Origin_Date = DateTime.UtcNow,
                Tiempo = DateTime.UtcNow.TimeOfDay
            };
            db.Historico.Add(historicoItem);
            db.SaveChanges();
        }

        public static Modelos.Evento ObtenerEventoSiguiente(Item item)
        {
            Evento Evento = item.Estado.Evento.Where(x => x.Estado_Final != 99).FirstOrDefault();

            return Evento;
        }

        // Aca tampoco
        public static void RegistrarHistoricoEstacion(Estacion_Trabajo estacion)
        {
            Modelos.Historico_Estacion_Trabajo historicoEstacion = new Modelos.Historico_Estacion_Trabajo
            {
                ID_Estacion_Trabajo = estacion.ID,
                ID_Estado_Estacion_Trabajo = estacion.ID_Estado_Trabajo,
                ID_Modo = estacion.Modo_ID_Figura,
                IsActive = true,
                Origin_Date = DateTime.UtcNow,
                Edit_Date = DateTime.UtcNow,
                Tiempo_Estacion_Trabajo = DateTime.UtcNow.TimeOfDay
            };
            db.Historico_Estacion_Trabajo.Add(historicoEstacion);
            db.SaveChanges();
        }


        // Setea la estacion en modo ocupado
        public static Estacion_Trabajo SetearEstacionOcupada(Estacion_Trabajo estacion)
        {
            estacion.ID_Estado_Trabajo = 2; // poner ocupado la estación
            db.SaveChanges();

            return estacion;
        }

        // Ves? No tiene sentido que lo explique
        public static Estacion_Trabajo SetearEstacionDisponible(Estacion_Trabajo estacion)
        {
            estacion.ID_Estado_Trabajo = 1; // poner disponible la estación
            db.SaveChanges();

            return estacion;
        }

        // Hace avanzar al item al estado siguiente
        public static Item CambiarEstadoItem(Item item, int estadoSiguiente)
        {
            item.ID_Estado = estadoSiguiente;
            db.SaveChanges();

            return item;
        }
        static public double CalcularArea(string Formula, double[] valores)
        {
            // Parsear el XML para obtener la fórmula
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Formula);
            string formula = doc.DocumentElement.InnerText;
            // Reemplazar los marcadores de posición en la fórmula con los valores
            for (int i = 0; i < valores.Length; i++)
            {
                formula = formula.Replace("{" + i + "}", valores[i].ToString());
            }
            // Evaluar la fórmula y devolver el resultado
            return EvaluarFormula(formula);
        }

        static public double EvaluarFormula(string formula)
        {
            try
            {
                // Utilizar la función Eval de C# para evaluar la expresión matemática
                return (double)new System.Xml.XPath.XPathDocument
                (new System.IO.StringReader("<r/>")).CreateNavigator().Evaluate
                ("number(" + new System.Text.RegularExpressions.Regex(@"([\+\-\*])").Replace(formula, " ${1} ").Replace("/", " div ").Replace("%", " mod ") + ")");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al evaluar la fórmula: " + ex.Message);
                return double.NaN;
            }
        }
        public static bool AvanzarFigura(int estacionID)   //Obtener Figura que actualmente está en la estacion
        {
            Item item = ItemController.ObtenerItemEstacion(estacionID);

            // Comprobar si el item es null (no hay figura en la estación)
            if (item == null)
            {
                return false;
            }

            //Obtenemos el evento que sigue
            Evento evento = ItemController.ObtenerEventoSiguiente(item);
            //Pasamos al siguiente estado el item
            item = ItemController.CambiarEstadoItem(item, evento.Estado_Final);
            //Registramos historico
            ItemController.RegistrarHistoricoItem(item, evento);

            //Pongo en disponible la estacion
            Estacion_Trabajo estacion = db.Estacion_Trabajo.FirstOrDefault(x => x.ID == estacionID);
            estacion = ItemController.SetearEstacionDisponible(estacion);
            //registrar historico estacion
            ItemController.RegistrarHistoricoEstacion(estacion);

            return true;

        }

        static public bool Siguiente(int EstacionID,List<Item> items)
        {
            int index;
            Estacion_Trabajo estacion = ObtenerEstacion(EstacionID);
            do
            {
                index = ListaMenu(estacion, items);
            } while (index == -1);
            if (index == -2) return false;
            Item item = items[index];

            if (!VerificarDisponibilidad(EstacionID, item.ID_Figura))
            {
                Console.WriteLine("Estacion de Trabajo no disponible Estado:" +estacion.Estado_Estacion_Trabajo.Estado);
                return false;
            }
            if (!VerificarModo(EstacionID, item.ID_Figura))
            {
                Console.WriteLine("Modo Erroneo necesita Modo: "+item.Figura.Figura1 + "Modo Actual: "+estacion.Figura.Figura1 );
                return false;
            }
            Estacion_Trabajo Upgrade_Estacion = SetearEstacionOcupada(estacion);
            Evento Next_Evento = ObtenerEventoSiguiente(item);
            int Next_Estado = Next_Evento.Estado_Final;
            Item Update_Item = CambiarEstadoItem(item, Next_Estado);
            RegistrarHistoricoItem(Update_Item,Next_Evento);
            RegistrarHistoricoEstacion(Upgrade_Estacion);

            return true;
        }
        static public int ListaMenu(Estacion_Trabajo estacion, List<Item> items)
        {
            Console.Clear();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("     Menu Estacion de Trabajo "+estacion.Nombre);
            Console.WriteLine("           Modo Actual: " + estacion.Figura.Figura1);
            Console.WriteLine("         Lista de Figuras en Espera");
            Console.WriteLine("--------------------------------------------");
            foreach (Item i in items)
            {
                Console.WriteLine($"|{i.ID}|  {i.Figura.Figura1} : {i.Color.Color1} ");
            }
            Console.WriteLine("Ingrese ID de Figura Seleccionada o ingrese 0 para salir: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                if (id == 0) return -2;
                int index = items.FindIndex(item => item.ID == id);
                if (index != -1)  return index;
                else return -1;
            }
            else
            {
                return -1;
            }
        }

        static public void Scrap(int EstacionID)
        {
            Estacion_Trabajo estacion = ObtenerEstacion(EstacionID);

            Item item = ObtenerItemEstacion(EstacionID);

            // No uso la funcion de ObtenerEvento porque no jala con el estado final 99
            Evento eventoActual = ObtenerEventoScrap(item);

            // Cambio el estado del item a scrap
            int estadoScrap = 99; // Sugerencia de Martin
            CambiarEstadoItem(item, estadoScrap);

            // Registro el item en el historico antes de cambiar su estado
            RegistrarHistoricoItem(item, eventoActual);

            // Seteo la estacion como disponible
            // Registro el cambio de estado en el historico
            RegistrarHistoricoEstacion(SetearEstacionDisponible(estacion));
        }

        static public void Cancelar(int EstacionID)
        {
            Item item = ObtenerItemEstacion(EstacionID);

            Evento eventoActual = ObtenerEvento(item);

            // Cambiar el estado del item a scrap
            int estadoCancelado = 100; // Sugerencia de Martin
            CambiarEstadoItem(item, estadoCancelado);

            // Registrar el item en el historico antes de cambiar su estado
            RegistrarHistoricoItem(item, eventoActual);
        }
    }
}

