using LibreriaComun.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaComun.DataContext
{
    public class DataContext
    {
        private static DataContext instancia;
        private static DBLPFEntities _context;

        // Constructor privado para evitar la creación de instancias desde fuera de la clase
        private DataContext()
        {
            _context = new DBLPFEntities();
        }

        // Método estático público para obtener la instancia única de la clase
        public static DBLPFEntities ObtenerInstancia()
        {
            // Si la instancia aún no ha sido creada, la creamos
            if (instancia == null)
            {
                instancia = new DataContext();
            }
            // Devolvemos la instancia única
            return _context;
        }
    }
}
