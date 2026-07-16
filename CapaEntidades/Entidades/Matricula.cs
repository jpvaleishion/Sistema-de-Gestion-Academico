using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa la matrícula de un estudiante en una asignatura, curso, docente y periodo específicos.
    /// </summary>
    public class Matricula
    {
        /// <summary>
        /// Identificador único de la matrícula.
        /// </summary>
        public int IdMatricula { get; set; }

        /// <summary>
        /// Identificador del estudiante matriculado.
        /// </summary>
        public int IdEstudiante { get; set; }

        /// <summary>
        /// Identificador de la asignatura asociada a la matrícula.
        /// </summary>
        public int IdAsignatura { get; set; }

        /// <summary>
        /// Identificador del docente asignado a la matrícula.
        /// </summary>
        public int IdDocente { get; set; }

        /// <summary>
        /// Identificador del curso asociado a la matrícula.
        /// </summary>
        public int IdCurso { get; set; }

        /// <summary>
        /// Identificador del periodo académico correspondiente a la matrícula.
        /// </summary>
        public int IdPeriodo { get; set; }

        /// <summary>
        /// Fecha en que se realizó la matrícula.
        /// </summary>
        public DateTime FechaMatricula { get; set; }

        /// <summary>
        /// Estado actual de la matrícula (por ejemplo, activa o retirada).
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Matricula"/>.
        /// </summary>
        public Matricula() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Matricula"/> con los datos especificados.
        /// </summary>
        /// <param name="idMatricula">Identificador único de la matrícula.</param>
        /// <param name="idEstudiante">Identificador del estudiante matriculado.</param>
        /// <param name="idAsignatura">Identificador de la asignatura asociada.</param>
        /// <param name="idDocente">Identificador del docente asignado.</param>
        /// <param name="idCurso">Identificador del curso asociado.</param>
        /// <param name="idPeriodo">Identificador del periodo académico correspondiente.</param>
        /// <param name="fechaMatricula">Fecha en que se realizó la matrícula.</param>
        /// <param name="estado">Estado actual de la matrícula.</param>
        public Matricula(int idMatricula, int idEstudiante, int idAsignatura, int idDocente,
            int idCurso, int idPeriodo, DateTime fechaMatricula, string estado)
        {
            IdMatricula = idMatricula;
            IdEstudiante = idEstudiante;
            IdAsignatura = idAsignatura;
            IdDocente = idDocente;
            IdCurso = idCurso;
            IdPeriodo = idPeriodo;
            FechaMatricula = fechaMatricula;
            Estado = estado;
        }
    }
}