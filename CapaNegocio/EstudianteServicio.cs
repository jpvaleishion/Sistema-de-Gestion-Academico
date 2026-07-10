using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class EstudianteServicio
    {
        EstudianteRepositorio repositorio = new EstudianteRepositorio();

        // RN-01: Nombres, Apellidos, Email y CodigoEstudiante son obligatorios
        public void Guardar(Estudiante e)
        {
            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");

            repositorio.Insertar(e);
        }

        public void Actualizar(Estudiante e)
        {
            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");

            repositorio.Actualizar(e);
        }

        public void Eliminar(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del estudiante no es válido.");

            repositorio.Eliminar(idPersona);
        }

        public List<Estudiante> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Estudiante ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del estudiante no es válido.");

            return repositorio.ObtenerPorId(idPersona);
        }
    }
}