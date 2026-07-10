using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class DocenteServicio
    {
        DocenteRepositorio repositorio = new DocenteRepositorio();

        // RN-02: No se puede registrar un docente sin especialidad
        public void Guardar(Docente d)
        {
            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            repositorio.Insertar(d);
        }

        public void Actualizar(Docente d)
        {
            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            repositorio.Actualizar(d);
        }

        public void Eliminar(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            repositorio.Eliminar(idPersona);
        }

        public List<Docente> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Docente ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            return repositorio.ObtenerPorId(idPersona);
        }
    }
}