using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Clase base abstracta que representa los datos generales comunes a toda persona registrada en el sistema.
    /// </summary>
    public abstract class Persona
    {
        /// <summary>
        /// Identificador único de la persona.
        /// </summary>
        public int IdPersona { get; set; }

        /// <summary>
        /// Nombres de la persona.
        /// </summary>
        public string Nombres { get; set; }

        /// <summary>
        /// Apellidos de la persona.
        /// </summary>
        public string Apellidos { get; set; }

        /// <summary>
        /// Correo electrónico de contacto de la persona.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Número de teléfono de contacto de la persona.
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Estado actual de la persona (por ejemplo, activo o inactivo).
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Persona"/>.
        /// </summary>
        public Persona() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Persona"/> con los datos especificados.
        /// </summary>
        /// <param name="idPersona">Identificador único de la persona.</param>
        /// <param name="nombres">Nombres de la persona.</param>
        /// <param name="apellidos">Apellidos de la persona.</param>
        /// <param name="email">Correo electrónico de contacto.</param>
        /// <param name="telefono">Número de teléfono de contacto.</param>
        /// <param name="estado">Estado actual de la persona.</param>
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