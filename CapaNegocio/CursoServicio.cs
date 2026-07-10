using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CursoServicio
    {
        CursoRepositorio repositorio = new CursoRepositorio();

        public void Guardar(Curso c)
        {
            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.Capacidad < 1)
                throw new ArgumentException("La capacidad del curso debe ser mayor a 0.");

            repositorio.Insertar(c);
        }

        public void Actualizar(Curso c)
        {
            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.Capacidad < 1)
                throw new ArgumentException("La capacidad del curso debe ser mayor a 0.");

            repositorio.Actualizar(c);
        }

        public void Eliminar(int idCurso)
        {
            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            repositorio.Eliminar(idCurso);
        }

        public List<Curso> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Curso ObtenerPorId(int idCurso)
        {
            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            return repositorio.ObtenerPorId(idCurso);
        }
    }
}