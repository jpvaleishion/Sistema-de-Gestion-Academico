using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class MatriculaServicio
    {
        MatriculaRepositorio repositorio = new MatriculaRepositorio();

        // RN-05: La matrícula debe estar asociada a Estudiante, Asignatura, Docente, Curso y Período
        public void Guardar(Matricula m)
        {
            if (m.IdEstudiante <= 0)
                throw new ArgumentException("Debe seleccionar un estudiante.");

            if (m.IdAsignatura <= 0)
                throw new ArgumentException("Debe seleccionar una asignatura.");

            if (m.IdDocente <= 0)
                throw new ArgumentException("Debe seleccionar un docente.");

            if (m.IdCurso <= 0)
                throw new ArgumentException("Debe seleccionar un curso.");

            if (m.IdPeriodo <= 0)
                throw new ArgumentException("Debe seleccionar un período académico.");

            repositorio.Insertar(m);
        }

        public void Actualizar(Matricula m)
        {
            if (m.IdEstudiante <= 0)
                throw new ArgumentException("Debe seleccionar un estudiante.");

            if (m.IdAsignatura <= 0)
                throw new ArgumentException("Debe seleccionar una asignatura.");

            if (m.IdDocente <= 0)
                throw new ArgumentException("Debe seleccionar un docente.");

            if (m.IdCurso <= 0)
                throw new ArgumentException("Debe seleccionar un curso.");

            if (m.IdPeriodo <= 0)
                throw new ArgumentException("Debe seleccionar un período académico.");

            repositorio.Actualizar(m);
        }

        public void Eliminar(int idMatricula)
        {
            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            repositorio.Eliminar(idMatricula);
        }

        public List<Matricula> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Matricula ObtenerPorId(int idMatricula)
        {
            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            return repositorio.ObtenerPorId(idMatricula);
        }
    }
}