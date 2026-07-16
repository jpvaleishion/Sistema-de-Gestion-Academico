using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa la calificación obtenida por un estudiante en una matrícula específica.
    /// </summary>
    public class Calificacion
    {
        /// <summary>
        /// Identificador único de la calificación.
        /// </summary>
        public int IdCalificacion { get; set; }

        /// <summary>
        /// Identificador de la matrícula asociada a la calificación.
        /// </summary>
        public int IdMatricula { get; set; }

        /// <summary>
        /// Primera nota parcial registrada.
        /// </summary>
        public decimal Nota1 { get; set; }

        /// <summary>
        /// Segunda nota parcial registrada.
        /// </summary>
        public decimal Nota2 { get; set; }

        /// <summary>
        /// Nota final resultante del proceso de evaluación.
        /// </summary>
        public decimal NotaFinal { get; set; }

        /// <summary>
        /// Nota máxima posible dentro de la escala de calificación.
        /// </summary>
        public decimal NotaMaxima { get; set; }

        /// <summary>
        /// Número de faltas o inasistencias registradas por el estudiante.
        /// </summary>
        public int Faltas { get; set; }

        /// <summary>
        /// Observaciones adicionales relacionadas con la calificación.
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// Fecha en la que se registró la calificación.
        /// </summary>
        public DateTime FechaCalificacion { get; set; }

        /// <summary>
        /// Estado actual de la calificación (por ejemplo, aprobado o reprobado).
        /// </summary>
        public String Estado { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Calificacion"/>.
        /// </summary>
        public Calificacion() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Calificacion"/> con los datos especificados.
        /// </summary>
        /// <param name="idCalificacion">Identificador único de la calificación.</param>
        /// <param name="idMatricula">Identificador de la matrícula asociada.</param>
        /// <param name="nota1">Primera nota parcial registrada.</param>
        /// <param name="nota2">Segunda nota parcial registrada.</param>
        /// <param name="notaFinal">Nota final resultante del proceso de evaluación.</param>
        /// <param name="notaMaxima">Nota máxima posible dentro de la escala de calificación.</param>
        /// <param name="faltas">Número de faltas o inasistencias registradas.</param>
        /// <param name="observaciones">Observaciones adicionales relacionadas con la calificación.</param>
        /// <param name="fechaCalificacion">Fecha en la que se registró la calificación.</param>
        /// <param name="estado">Estado actual de la calificación.</param>
        public Calificacion(int idCalificacion,
            int idMatricula, decimal nota1, decimal nota2, decimal notaFinal, decimal notaMaxima,
            int faltas, string observaciones, DateTime fechaCalificacion, string estado)
        {
            IdCalificacion = idCalificacion;
            IdMatricula = idMatricula;
            Nota1 = nota1;
            Nota2 = nota2;
            NotaFinal = notaFinal;
            NotaMaxima = notaMaxima;
            Faltas = faltas;
            Observaciones = observaciones;
            FechaCalificacion = fechaCalificacion;
            Estado = estado;
        }
    }
}