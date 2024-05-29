using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaComun.Clases
{
    internal class ItemController
    {
        // Retorna el estado siguiente de un item
        public static int ObtenerEstadoSiguiente(Item item)
        {
            Evento evento = item.Estado.Evento.Where(x => x.Estado_Final != 99).FirstOrDefault();
            return evento != null ? evento.Estado_Final : -1;
        }

        // Verifica disponibilidad de una estacion
        public static bool VerificarDisponibilidadYModo(int estacionSiguiente, int idFigura)
        {
            Modelos.Estacion_Trabajo estacion = db.Estacion_Trabajo.Where(x => x.ID == estacionSiguiente).FirstOrDefault();
            return estacion != null && estacion.ID_Estado_Trabajo == 1 && estacion.Modo_ID_Figura == idFigura;
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

        // Aca tampoco
        public static void RegistrarHistoricoEstacion(Modelos.Estacion_Trabajo estacion)
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
        public static void SetearEstacionOcupada(Modelos.Estacion_Trabajo estacion)
        {
            estacion.ID_Estado_Trabajo = 2; // poner ocupado la estación
            db.SaveChanges();
        }

        // Ves? No tiene sentido que lo explique
        public static void SetearEstacionDisponible(Modelos.Estacion_Trabajo estacion)
        {
            estacion.ID_Estado_Trabajo = 1; // poner disponible la estación
            db.SaveChanges();
        }

        // Hace avanzar al item al estado siguiente
        public static void CambiarEstadoItem(Item item, int estadoSiguiente)
        {
            item.ID_Estado = estadoSiguiente;
            db.SaveChanges();
        }
    }
}
