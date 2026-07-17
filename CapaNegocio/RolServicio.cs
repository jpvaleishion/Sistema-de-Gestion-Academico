using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de roles.
    /// Proporciona operaciones de lectura sobre los roles almacenados.
    /// </summary>
    public class RolServicio
    {
        private RolRepositorio repositorio = new RolRepositorio();

        /// <summary>
        /// Obtiene la lista completa de roles procesada desde el repositorio.
        /// </summary>
        public List<Rol> ObtenerTodos()
        {
            try
            {
                var roles = repositorio.ObtenerTodos();
                if (roles == null || roles.Count == 0)
                    throw new InvalidOperationException("No se encontraron roles registrados en el sistema.");

                return roles;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener los roles.", ex);
            }
        }

        /// <summary>
        /// Obtiene un rol por su identificador.
        /// </summary>
        /// <param name="idRol">Identificador del rol.</param>
        /// <returns>Entidad Rol correspondiente.</returns>
        public Rol ObtenerPorId(int idRol)
        {
            if (idRol <= 0)
                throw new ArgumentException("El identificador del rol no es válido.");

            try
            {
                var rol = repositorio.ObtenerPorId(idRol);
                if (rol == null)
                    throw new InvalidOperationException($"No existe un rol con ID {idRol}.");

                return rol;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el rol.", ex);
            }
        }
    }
}
