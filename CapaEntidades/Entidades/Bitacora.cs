using System;

namespace CapaEntidades.Entidades
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Modulo { get; set; }
        public string Accion { get; set; }
        public string Descripcion { get; set; }
        public string NombreUsuario { get; set; }
    }
}