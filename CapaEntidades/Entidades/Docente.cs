using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa a un docente dentro del sistema, extendiendo los datos generales de <see cref="Persona"/>.
    /// </summary>
    public class Docente : Persona
    {
        /// <summary>
        /// Área o especialidad académica del docente.
        /// </summary>
        public string Especialidad { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Docente"/>.
        /// </summary>
        public Docente() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Docente"/> con los datos especificados.
        /// </summary>
        /// <param name="idPersona">Identificador único de la persona.</param>
        /// <param name="nombres">Nombres de la persona.</param>
        /// <param name="apellidos">Apellidos de la persona.</param>
        /// <param name="email">Correo electrónico de contacto.</param>
        /// <param name="telefono">Número de teléfono de contacto.</param>
        /// <param name="estado">Estado actual de la persona.</param>
        /// <param name="especialidad">Área o especialidad académica del docente.</param>
        public Docente(int idPersona, string nombres, string apellidos, string email, string telefono, string estado, string especialidad) :
            base(idPersona, nombres, apellidos, email, telefono, estado)
        {
            especialidad = Especialidad;
        }
    }
}