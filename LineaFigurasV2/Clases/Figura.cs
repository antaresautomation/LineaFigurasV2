using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineaFigurasV2.Clases
{
    public abstract class Figura
    {
        private static int nextID = 1;
        public int ID { get; }

        public string Color { get; set; }
        public string Type { get; }
        public string EstacionActual { get; set; }

        public Figura(string tipo)
        {
            ID = nextID++;
            Type = tipo;
            EstacionActual = "Creando";

        }
    }
}
