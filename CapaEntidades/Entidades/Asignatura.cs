using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Asignatura
    {
        public Asignatura() { }

        public int IdAsignatura { get; set; }
        public string Nombre { get; set; }
        public int Creditos { get; set; }
        public string Modalidad { get; set; }
        public string Estado { get; set; }

        public Asignatura(int idAsignatura, string nombre, int creditos, string modalidad, string estado)
        {
            IdAsignatura = idAsignatura;
            Nombre = nombre;
            Creditos = creditos;
            Modalidad = modalidad;
            Estado = estado;
        }
    }
}
