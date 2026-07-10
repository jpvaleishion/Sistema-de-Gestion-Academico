using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Estudiante : Persona
    {
        public Estudiante() { }
        
        public string CodigoEstudiante { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Tipo { get; set; }

        public Estudiante(int idPersona, string nombres, string apellidos, string email, string telefono, string estado,
           string codigoEstudiante, DateTime fechaInscripcion, string tipo)
           : base(idPersona, nombres, apellidos, email, telefono, estado)
        {
            CodigoEstudiante = codigoEstudiante;
            FechaInscripcion = fechaInscripcion;
            Tipo = tipo;
        }
    }
}
