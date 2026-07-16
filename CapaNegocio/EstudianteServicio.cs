using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de estudiantes.
    /// Aplica reglas de negocio, validaciones y registra auditoría mediante bitácora y permisos.
    /// </summary>
    public class EstudianteServicio
    {
        private EstudianteRepositorio repositorio = new EstudianteRepositorio();
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Registra un nuevo estudiante aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="e">Entidad Estudiante a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Guardar(Estudiante e, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar estudiantes.");
            }

            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");

            repositorio.Insertar(e);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Estudiantes",
                "Crear",
                $"Se registró al estudiante: {e.Nombres} {e.Apellidos} (Código: {e.CodigoEstudiante})."
            );
        }

        /// <summary>
        /// Actualiza los datos de un estudiante aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="e">Entidad Estudiante a actualizar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Actualizar(Estudiante e, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar estudiantes.");
            }

            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");

            repositorio.Actualizar(e);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Estudiantes",
                "Modificar",
                $"Se actualizaron los datos del estudiante ID: {e.IdPersona} ({e.Nombres} {e.Apellidos}, Código: {e.CodigoEstudiante})."
            );
        }

        /// <summary>
        /// Elimina un estudiante del sistema y registra la acción en la bitácora.
        /// </summary>
        /// <param name="idPersona">Identificador de la persona/estudiante a eliminar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Eliminar(int idPersona, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar estudiantes.");
            }

            if (idPersona <= 0)
                throw new ArgumentException("El identificador del estudiante no es válido.");

            string nombreEstudiante = $"ID {idPersona}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idPersona);
                if (de_paso != null)
                {
                    nombreEstudiante = $"'{de_paso.Nombres} {de_paso.Apellidos}' (ID: {idPersona})";
                }
            }
            catch { }

            repositorio.Eliminar(idPersona);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Estudiantes",
                "Eliminar",
                $"Se eliminó al estudiante: {nombreEstudiante}."
            );
        }

        /// <summary>
        /// Obtiene todos los estudiantes registrados.
        /// </summary>
        /// <returns>Lista de estudiantes.</returns>
        public List<Estudiante> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un estudiante por su identificador.
        /// </summary>
        /// <param name="idPersona">Identificador del estudiante.</param>
        /// <returns>Entidad Estudiante correspondiente.</returns>
        public Estudiante ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del estudiante no es válido.");

            return repositorio.ObtenerPorId(idPersona);
        }
    }
}
