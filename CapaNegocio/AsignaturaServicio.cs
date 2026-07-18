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
    /// Implementa registro de errores en la bitácora sin cambiar la firma existente.
    /// </summary>
    public class AsignaturaServicio
    {
        private readonly AsignaturaRepositorio repositorio = new AsignaturaRepositorio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();
        private readonly PermisoServicio permisoService = new PermisoServicio();

        /// <summary>
        /// Método auxiliar para validar asignatura.
        /// </summary>
        private void ValidarAsignatura(Asignatura a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a), "La asignatura no puede ser nula.");

            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Nombre.Length < 3 || a.Nombre.Length > 50)
                throw new ArgumentException("El nombre de la asignatura debe tener entre 3 y 50 caracteres.");

            // Permitir letras Unicode (acentos, ñ, etc.) y espacios
            if (!Regex.IsMatch(a.Nombre, @"^[\p{L}\s]+$"))
                throw new ArgumentException("El nombre de la asignatura solo puede contener letras y espacios.");

            if (a.Creditos < 1 || a.Creditos > 10)
                throw new ArgumentException("La asignatura debe tener entre 1 y 10 créditos.");
        }

        /// <summary>
        /// Registra un error en la bitácora. No modifica la estructura de la bitácora existente.
        /// </summary>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacoraService.RegistrarAccion(idUsuario, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde el registro de errores para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra una nueva asignatura en el sistema aplicando validaciones y auditoría.
        /// </summary>
        public void Guardar(Asignatura a, int idUsuarioLogueado)
        {
            try
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
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Asignaturas",
                    "Crear",
                    $"Intentando guardar asignatura: Nombre='{(a != null ? a.Nombre : "null")}', Créditos={(a != null ? a.Creditos.ToString() : "null")}"
                );
                throw;
            }
        }

        /// <summary>
        /// Actualiza los datos de una asignatura existente aplicando validaciones y auditoría.
        /// </summary>
        public void Actualizar(Asignatura a, int idUsuarioLogueado)
        {
            try
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
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Asignaturas",
                    "Modificar",
                    $"Intentando actualizar asignatura ID={(a != null ? a.IdAsignatura.ToString() : "null")}"
                );
                throw;
            }
        }

        /// <summary>
        /// Elimina una asignatura del sistema y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idAsignatura, int idUsuarioLogueado)
        {
            try
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
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Asignaturas", "Eliminar", $"Obteniendo nombre para ID={idAsignatura}");
                }

                repositorio.Eliminar(idAsignatura);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Asignaturas",
                    "Eliminar",
                    $"Se eliminó la asignatura: {nombreAsignatura}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, idUsuarioLogueado, "Asignaturas", "Eliminar", $"Intentando eliminar asignatura ID={idAsignatura}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las asignaturas registradas.
        /// </summary>
        public List<Asignatura> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Asignaturas", "ObtenerTodos", "Obteniendo todas las asignaturas");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una asignatura por su identificador.
        /// </summary>
        public Asignatura ObtenerPorId(int idAsignatura)
        {
            try
            {
                if (idAsignatura <= 0)
                    throw new ArgumentException("El identificador de la asignatura no es válido.");

                return repositorio.ObtenerPorId(idAsignatura);
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Asignaturas", "ObtenerPorId", $"Obteniendo asignatura ID={idAsignatura}");
                throw;
            }
        }
    }
}
