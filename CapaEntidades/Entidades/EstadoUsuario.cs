using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class EstadoUsuario
    {
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public string Descripcion { get; set; }

        // Constructor vacío por defecto (buena práctica para mapeos)
        public EstadoUsuario() { }

        // Constructor opcional por comodidad
        public EstadoUsuario(int idEstado, string nombreEstado, string descripcion)
        {
            IdEstado = idEstado;
            NombreEstado = nombreEstado;
            Descripcion = descripcion;
        }
    }
}
