using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio responsable de la resolución de permisos del sistema.
    /// Provee operaciones para consultar permisos por rol y verificar permisos concretos de usuario.
    /// Registra errores en la bitácora sin cambiar las firmas públicas.
    /// </summary>
    public class PermisoServicio
    {
        private readonly PermisoRepositorio repositorio = new PermisoRepositorio();
        private readonly BitacoraServicio bitacora = new BitacoraServicio();

        /// <summary>
        /// Obtiene la lista de permisos asociados a un rol específico.
        /// Registra en bitácora si ocurre un error y envuelve la excepción para mantener comportamiento consistente.
        /// </summary>
        /// <param name="idRol">Identificador del rol cuyos permisos se desean obtener.</param>
        /// <returns>Lista de <see cref="Permiso"/> asociados al rol.</returns>
        /// <exception cref="ArgumentException">Si <paramref name="idRol"/> no es un valor válido (&lt;= 0).</exception>
        /// <exception cref="InvalidOperationException">Si no se encuentran permisos o ocurre un error al acceder al repositorio.</exception>
        public List<Permiso> ObtenerPermisosPorRol(int idRol)
        {
            if (idRol <= 0)
                throw new ArgumentException("El identificador del rol no es válido.", nameof(idRol));

            try
            {
                var permisos = repositorio.ObtenerPermisosPorRol(idRol);
                if (permisos == null || permisos.Count == 0)
                {
                    var ex = new InvalidOperationException($"No se encontraron permisos asociados al rol ID {idRol}.");
                    RegistrarErrorEnBitacora(ex, 0, "Permisos", "ObtenerPermisosPorRol", $"RolID={idRol}");
                    throw ex;
                }

                return permisos;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Permisos", "ObtenerPermisosPorRol", $"Obteniendo permisos para RolID={idRol}");
                throw new InvalidOperationException("Error al obtener permisos por rol.", ex);
            }
        }

        /// <summary>
        /// Verifica si un usuario tiene un permiso concreto sobre un formulario y operación.
        /// En caso de error devuelve false y registra el incidente en la bitácora.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a verificar.</param>
        /// <param name="nombreFormulario">Nombre lógico del formulario (p.ej. "frmEstudiantes").</param>
        /// <param name="operacion">Operación requerida (p.ej. "Crear", "Modificar").</param>
        /// <returns>True si el usuario tiene el permiso; false en caso contrario o si ocurre un error durante la verificación.</returns>
        /// <remarks>Devuelve false ante cualquier error para evitar conceder acceso por fallo.</remarks>
        public bool TienePermiso(int idUsuario, string nombreFormulario, string operacion)
        {
            if (idUsuario <= 0) return false;
            if (string.IsNullOrWhiteSpace(nombreFormulario)) return false;
            if (string.IsNullOrWhiteSpace(operacion)) return false;

            try
            {
                return repositorio.TienePermiso(idUsuario, nombreFormulario.Trim(), operacion.Trim());
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, idUsuario, "Permisos", "TienePermiso", $"UsuarioID={idUsuario}, Formulario='{nombreFormulario}', Operacion='{operacion}'");
                // En caso de fallo al verificar permisos, devolvemos false para no conceder acceso por error.
                return false;
            }
        }

        /// <summary>
        /// Registra un error en la bitácora. Usa idUsuario = 0 cuando no se dispone de usuario.
        /// </summary>
        /// <param name="ex">Excepción capturada.</param>
        /// <param name="idUsuario">Identificador del usuario relacionado a la acción (0 si no aplica).</param>
        /// <param name="modulo">Módulo origen del error.</param>
        /// <param name="accion">Acción en la que ocurrió el error.</param>
        /// <param name="contexto">Contexto adicional para diagnóstico.</param>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacora.RegistrarAccion(idUsuario <= 0 ? 0 : idUsuario, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde la bitácora para no afectar la lógica de negocio.
            }
        }
    }
}
