using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa una asignatura académica ofertada dentro del sistema de gestión escolar.
    /// </summary>
    public class Asignatura
    {
        /// <summary>
        /// Identificador único de la asignatura.
        /// </summary>
        public int IdAsignatura { get; set; }

        /// <summary>
        /// Nombre descriptivo de la asignatura.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Número de créditos académicos que otorga la asignatura.
        /// </summary>
        public int Creditos { get; set; }

        /// <summary>
        /// Modalidad de impartición de la asignatura (por ejemplo, presencial, virtual o híbrida).
        /// </summary>
        public string Modalidad { get; set; }

        /// <summary>
        /// Estado actual de la asignatura (por ejemplo, activa o inactiva).
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Asignatura"/>.
        /// </summary>
        public Asignatura() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Asignatura"/> con los datos especificados.
        /// </summary>
        /// <param name="idAsignatura">Identificador único de la asignatura.</param>
        /// <param name="nombre">Nombre descriptivo de la asignatura.</param>
        /// <param name="creditos">Número de créditos académicos que otorga la asignatura.</param>
        /// <param name="modalidad">Modalidad de impartición de la asignatura.</param>
        /// <param name="estado">Estado actual de la asignatura.</param>
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