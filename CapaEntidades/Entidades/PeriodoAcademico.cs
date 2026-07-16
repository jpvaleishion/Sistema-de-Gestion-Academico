using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa un periodo académico dentro del cual se desarrollan las actividades escolares.
    /// </summary>
    public class PeriodoAcademico
    {
        /// <summary>
        /// Identificador único del periodo académico.
        /// </summary>
        public int IdPeriodo { get; set; }

        /// <summary>
        /// Nombre descriptivo del periodo académico.
        /// </summary>
        public string NombrePeriodo { get; set; }

        /// <summary>
        /// Fecha de inicio del periodo académico.
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Fecha de finalización del periodo académico.
        /// </summary>
        public DateTime FechaFin { get; set; }

        /// <summary>
        /// Estado actual del periodo académico (por ejemplo, activo o cerrado).
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="PeriodoAcademico"/>.
        /// </summary>
        public PeriodoAcademico() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PeriodoAcademico"/> con los datos especificados.
        /// </summary>
        /// <param name="idPeriodo">Identificador único del periodo académico.</param>
        /// <param name="nombrePeriodo">Nombre descriptivo del periodo académico.</param>
        /// <param name="fechaInicio">Fecha de inicio del periodo académico.</param>
        /// <param name="fechaFin">Fecha de finalización del periodo académico.</param>
        /// <param name="estado">Estado actual del periodo académico.</param>
        public PeriodoAcademico(int idPeriodo, string nombrePeriodo, DateTime fechaInicio, DateTime fechaFin, string estado)
        {
            IdPeriodo = idPeriodo;
            NombrePeriodo = nombrePeriodo;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Estado = estado;
        }
    }
}