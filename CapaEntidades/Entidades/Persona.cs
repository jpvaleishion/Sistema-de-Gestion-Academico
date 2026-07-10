using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public abstract class Persona
    {
        public Persona() { }

        public int IdPersona { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }

        protected Persona(int idPersona, string nombres, string apellidos, string email, string telefono, string estado)
        {
            IdPersona = idPersona;
            Nombres = nombres;
            Apellidos = apellidos;
            Email = email;
            Telefono = telefono;
            Estado = estado;
        }

        
    }
}
