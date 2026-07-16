using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa a un estudiante dentro del sistema, extendiendo los datos generales de <see cref="Persona"/>.
    /// </summary>
    public class Estudiante : Persona
    {
        /// <summary>
        /// Código único de identificación del estudiante.
        /// </summary>
        public string CodigoEstudiante { get; set; }

        /// <summary>
        /// Fecha en que el estudiante se inscribió en el sistema.
        /// </summary>
        public DateTime FechaInscripcion { get; set; }

        /// <summary>
        /// Tipo de estudiante (por ejemplo, regular o especial).
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Estudiante"/>.
        /// </summary>
        public Estudiante() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Estudiante"/> con los datos especificados.
        /// </summary>
        /// <param name="idPersona">Identificador único de la persona.</param>
        /// <param name="nombres">Nombres de la persona.</param>
        /// <param name="apellidos">Apellidos de la persona.</param>
        /// <param name="email">Correo electrónico de contacto.</param>
        /// <param name="telefono">Número de teléfono de contacto.</param>
        /// <param name="estado">Estado actual de la persona.</param>
        /// <param name="codigoEstudiante">Código único de identificación del estudiante.</param>
        /// <param name="fechaInscripcion">Fecha en que el estudiante se inscribió en el sistema.</param>
        /// <param name="tipo">Tipo de estudiante.</param>
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