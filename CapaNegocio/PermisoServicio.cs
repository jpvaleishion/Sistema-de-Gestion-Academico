using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio responsable de la resolución de permisos del sistema.
    /// Provee operaciones para consultar permisos por rol y verificar permisos concretos de usuario.
    /// </summary>
    public class PermisoServicio
    {
        private PermisoRepositorio repositorio = new PermisoRepositorio();

        /// <summary>
        /// Obtiene la lista de permisos asociados a un rol específico.
        /// </summary>
        public List<Permiso> ObtenerPermisosPorRol(int idRol)
        {
            if (idRol <= 0)
                throw new ArgumentException("El identificador del rol no es válido.");

            try
            {
                var permisos = repositorio.ObtenerPermisosPorRol(idRol);
                if (permisos == null || permisos.Count == 0)
                    throw new InvalidOperationException($"No se encontraron permisos asociados al rol ID {idRol}.");

                return permisos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener permisos por rol.", ex);
            }
        }

        /// <summary>
        /// Verifica si un usuario tiene un permiso concreto sobre un formulario y operación.
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
                throw new InvalidOperationException("Error al verificar permisos del usuario.", ex);
            }
        }
    }
}
