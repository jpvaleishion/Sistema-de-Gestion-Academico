using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        /// Método auxiliar para validar asignatura.
        /// </summary>
        private void ValidarAsignatura(Asignatura a)
        {
            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Nombre.Length < 3 || a.Nombre.Length > 50)
                throw new ArgumentException("El nombre de la asignatura debe tener entre 3 y 50 caracteres.");

            if (!Regex.IsMatch(a.Nombre, @"^[a-zA-Z\s]+$"))
                throw new ArgumentException("El nombre de la asignatura solo puede contener letras y espacios.");

            if (a.Creditos < 1 || a.Creditos > 10)
                throw new ArgumentException("La asignatura debe tener entre 1 y 10 créditos.");
        }

        /// <summary>
        /// Registra una nueva asignatura en el sistema aplicando validaciones y auditoría.
        /// </summary>
        public void Guardar(Asignatura a, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar asignaturas.");

            ValidarAsignatura(a);

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
        public void Actualizar(Asignatura a, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar asignaturas.");

            ValidarAsignatura(a);

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
        public void Eliminar(int idAsignatura, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Eliminar"))
                throw new InvalidOperationException("No tiene permisos para eliminar asignaturas.");

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
        public List<Asignatura> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene una asignatura por su identificador.
        /// </summary>
        public Asignatura ObtenerPorId(int idAsignatura)
        {
            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            return repositorio.ObtenerPorId(idAsignatura);
        }
    }
}
