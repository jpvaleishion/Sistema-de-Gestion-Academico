using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa un curso o grupo académico en el cual se agrupan los estudiantes.
    /// </summary>
    public class Curso
    {
        /// <summary>
        /// Identificador único del curso.
        /// </summary>
        public int IdCurso { get; set; }

        /// <summary>
        /// Nombre del curso.
        /// </summary>
        public string NombreCurso { get; set; }

        /// <summary>
        /// Paralelo o sección específica del curso.
        /// </summary>
        public string Paralelo { get; set; }

        /// <summary>
        /// Capacidad máxima de estudiantes permitida en el curso.
        /// </summary>
        public int Capacidad { get; set; }

        /// <summary>
        /// Estado actual del curso (por ejemplo, activo o inactivo).
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Curso"/>.
        /// </summary>
        public Curso() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Curso"/> con los datos especificados.
        /// </summary>
        /// <param name="idCurso">Identificador único del curso.</param>
        /// <param name="nombreCurso">Nombre del curso.</param>
        /// <param name="paralelo">Paralelo o sección específica del curso.</param>
        /// <param name="capacidad">Capacidad máxima de estudiantes permitida.</param>
        /// <param name="estado">Estado actual del curso.</param>
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