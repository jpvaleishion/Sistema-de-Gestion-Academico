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
