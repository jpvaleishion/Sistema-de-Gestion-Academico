using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Calificacion
    {
        public Calificacion() { }


        public int IdCalificacion { get; set; }
        public int IdMatricula { get; set; }
        public decimal Nota1 { get; set; }
        public decimal Nota2 { get; set; }
        public decimal NotaFinal { get; set; }
        public decimal NotaMaxima { get; set; }
        public int Faltas { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCalificacion { get; set; }
        public String Estado { get; set; }

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
