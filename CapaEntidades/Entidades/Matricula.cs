using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Matricula
    {
        public Matricula() { }

        public int IdMatricula { get; set; }
        public int IdEstudiante { get; set; }
        public int IdAsignatura { get; set; }
        public int IdDocente { get; set; }
        public int IdCurso { get; set; }
        public int IdPeriodo { get; set; }
        public DateTime FechaMatricula { get; set; }
        public string Estado { get; set; }

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
