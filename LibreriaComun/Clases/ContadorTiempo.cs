using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LibreriaComun.Clases
{

    public class ContadorTiempo
    {
        private readonly TimeSpan intervalo;
        private DateTime inicio;
        private Thread hilo;

        public event EventHandler<TimeSpan> TiempoTranscurrido;

        public ContadorTiempo(TimeSpan intervalo)
        {
            this.intervalo = intervalo;
        }

        public void Iniciar()
        {
            inicio = DateTime.UtcNow;
            hilo = new Thread(Contar);
            hilo.IsBackground = true;
            hilo.Start();
        }

        public void Detener()
        {
            if (hilo != null && hilo.IsAlive)
            {
                hilo.Abort();
            }
        }

        private void Contar()
        {
            while (true)
            {
                Thread.Sleep(intervalo); // Esperar el intervalo de tiempo

                // Calcular el tiempo transcurrido
                TimeSpan tiempoTranscurrido = DateTime.UtcNow - inicio;

                // Disparar el evento de tiempo transcurrido
                TiempoTranscurrido?.Invoke(this, tiempoTranscurrido);
            }
        }
    }

}
