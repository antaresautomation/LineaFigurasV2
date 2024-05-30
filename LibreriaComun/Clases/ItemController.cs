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
        public static int ObtenerEstadoSiguiente(Evento evento)
        {
            return evento != null ? evento.Estado_Final : -1;
        }
        public static Estacion_Trabajo ObtenerEstacion(int ID_Estacion_Trabajo)
        {
            Estacion_Trabajo estacionDyT =db.Estacion_Trabajo.Where(x => x.ID == ID_Estacion_Trabajo).FirstOrDefault();
            return estacionDyT;
        }

        // Verifica disponibilidad de una estacion
        public static bool VerificarDisponibilidadYModo(int estacionSiguiente, int idFigura)
        {
            
            Estacion_Trabajo estacion = ObtenerEstacion(estacionSiguiente);
            return estacion != null && estacion.ID_Estado_Trabajo == 1 && estacion.Modo_ID_Figura == idFigura;
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
                Edit_Date = DateTime.Now,
                Origin_Date = DateTime.Now,
                Tiempo = DateTime.Now.TimeOfDay
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
                Origin_Date = DateTime.Now,
                Edit_Date = DateTime.Now,
                Tiempo_Estacion_Trabajo = DateTime.Now.TimeOfDay
            };
            db.Historico_Estacion_Trabajo.Add(historicoEstacion);
            db.SaveChanges();
        }


        // Setea la estacion en modo ocupado
        public static void SetearEstacionOcupada(Estacion_Trabajo estacion)
        {
            estacion.ID_Estado_Trabajo = 2; // poner ocupado la estación
            db.SaveChanges();
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
        static public void Siguiente(int EstacionID,Item item)
        {
            Estacion_Trabajo estacion = ObtenerEstacion(EstacionID);
            Evento evento = ObtenerEvento(item);
            if (VerificarDisponibilidadYModo(EstacionID,item.ID_Figura))
            {
                
                SetearEstacionOcupada(estacion);
                CambiarEstadoItem(item, ObtenerEstadoSiguiente(evento));
                RegistrarHistoricoItem(item, evento);
                RegistrarHistoricoEstacion(estacion);
            }
            else
            {
                Console.WriteLine("WTF PP");
            }
        }
    }
}

