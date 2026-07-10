using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class PeriodoAcademico
    {
        public PeriodoAcademico() { }

        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }

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
