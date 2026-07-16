using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de asignaturas.
    /// Aplica reglas de negocio, validaciones y registra auditoría mediante bitácora y permisos.
    /// </summary>
    public class AsignaturaServicio
    {
        private AsignaturaRepositorio repositorio = new AsignaturaRepositorio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();
        private PermisoServicio permisoService = new PermisoServicio();

        /// <summary>
        /// Registra una nueva asignatura en el sistema aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="a">Entidad Asignatura a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Guardar(Asignatura a, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar asignaturas.");
            }

            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Creditos < 1)
                throw new ArgumentException("La asignatura debe tener al menos 1 crédito.");

            repositorio.Insertar(a);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Asignaturas",
                "Crear",
                $"Se registró la asignatura: '{a.Nombre}' con {a.Creditos} créditos."
            );
        }

        /// <summary>
        /// Actualiza los datos de una asignatura existente aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="a">Entidad Asignatura a actualizar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Actualizar(Asignatura a, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar asignaturas.");
            }

            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Creditos < 1)
                throw new ArgumentException("La asignatura debe tener al menos 1 crédito.");

            repositorio.Actualizar(a);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Asignaturas",
                "Modificar",
                $"Se actualizó la asignatura ID {a.IdAsignatura} a: '{a.Nombre}' ({a.Creditos} créditos)."
            );
        }

        /// <summary>
        /// Elimina una asignatura del sistema y registra la acción en la bitácora.
        /// </summary>
        /// <param name="idAsignatura">Identificador de la asignatura a eliminar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Eliminar(int idAsignatura, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar asignaturas.");
            }

            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            string nombreAsignatura = "ID " + idAsignatura;
            try
            {
                var de_paso = repositorio.ObtenerPorId(idAsignatura);
                if (de_paso != null) nombreAsignatura = $"'{de_paso.Nombre}' (ID: {idAsignatura})";
            }
            catch { }

            repositorio.Eliminar(idAsignatura);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Asignaturas",
                "Eliminar",
                $"Se eliminó la asignatura: {nombreAsignatura}."
            );
        }

        /// <summary>
        /// Obtiene todas las asignaturas registradas.
        /// </summary>
        /// <returns>Lista de asignaturas.</returns>
        public List<Asignatura> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene una asignatura por su identificador.
        /// </summary>
        /// <param name="idAsignatura">Identificador de la asignatura.</param>
        /// <returns>Entidad Asignatura correspondiente.</returns>
        public Asignatura ObtenerPorId(int idAsignatura)
        {
            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            return repositorio.ObtenerPorId(idAsignatura);
        }
    }
}
