using CapaDatos;
using CapaEntidades.Entidades;
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
        /// <returns>Lista de entidades <see cref="Rol"/> disponibles en el sistema.</returns>
        public List<Rol> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }
    }
}
