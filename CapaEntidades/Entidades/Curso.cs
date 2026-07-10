using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Curso
    {
        public Curso() { }

        public int IdCurso { get; set; }
        public string NombreCurso { get; set; }
        public string Paralelo { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; }

        public Curso(int idCurso, string nombreCurso, string paralelo, int capacidad, string estado)
        {
            IdCurso = idCurso;
            NombreCurso = nombreCurso;
            Paralelo = paralelo;
            Capacidad = capacidad;
            Estado = estado;
        }
    }
}
