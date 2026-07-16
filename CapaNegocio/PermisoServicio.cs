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
        /// <param name="idRol">Identificador del rol cuyo conjunto de permisos se desea recuperar.</param>
        /// <returns>Lista de entidades <see cref="Permiso"/> asociadas al rol.</returns>
        /// <exception cref="ArgumentException">Se lanza cuando <paramref name="idRol"/> es menor o igual a cero.</exception>
        public List<Permiso> ObtenerPermisosPorRol(int idRol)
        {
            if (idRol <= 0)
                throw new ArgumentException("El identificador del rol no es válido.");

            return repositorio.ObtenerPermisosPorRol(idRol);
        }

        /// <summary>
        /// Verifica si un usuario tiene un permiso concreto sobre un formulario y operación.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario que se valida.</param>
        /// <param name="nombreFormulario">Nombre del formulario o recurso sobre el que se solicita el permiso.</param>
        /// <param name="operacion">Operación requerida (Ej: Crear, Modificar, Eliminar, Consultar).</param>
        /// <returns>True si el usuario posee el permiso solicitado; False en caso contrario o si los parámetros son inválidos.</returns>
        public bool TienePermiso(int idUsuario, string nombreFormulario, string operacion)
        {
            // Validaciones rápidas de seguridad
            if (idUsuario <= 0) return false;
            if (string.IsNullOrWhiteSpace(nombreFormulario)) return false;
            if (string.IsNullOrWhiteSpace(operacion)) return false;

            return repositorio.TienePermiso(idUsuario, nombreFormulario, operacion);
        }
    }
}
