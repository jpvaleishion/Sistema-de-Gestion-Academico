using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Docente : Persona
    {
        public Docente() { }

      

        public string Especialidad { get; set; }

        public Docente(int idPersona, string nombres, string apellidos, string email, string telefono, string estado, string especialidad) : 
            base(idPersona, nombres, apellidos, email, telefono, estado)
        {
            especialidad = Especialidad;
        }
    }
}
