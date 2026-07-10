using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class AsignaturaServicio
    {
        AsignaturaRepositorio repositorio = new AsignaturaRepositorio();

        // RN-03: Una asignatura debe tener al menos 1 crédito
        public void Guardar(Asignatura a)
        {
            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Creditos < 1)
                throw new ArgumentException("La asignatura debe tener al menos 1 crédito.");

            repositorio.Insertar(a);
        }

        public void Actualizar(Asignatura a)
        {
            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Creditos < 1)
                throw new ArgumentException("La asignatura debe tener al menos 1 crédito.");

            repositorio.Actualizar(a);
        }

        public void Eliminar(int idAsignatura)
        {
            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            repositorio.Eliminar(idAsignatura);
        }

        public List<Asignatura> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Asignatura ObtenerPorId(int idAsignatura)
        {
            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            return repositorio.ObtenerPorId(idAsignatura);
        }
    }
}