using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para el catálogo de estados de usuario.
    /// </summary>
    public class EstadoUsuarioServicio
    {
        private readonly EstadoUsuarioRepositorio _repositorio = new EstadoUsuarioRepositorio();

        /// <summary>
        /// Obtiene todos los estados disponibles en el sistema para mapear en la interfaz de usuario.
        /// </summary>
        /// <returns>Una lista de objetos EstadoUsuario.</returns>
        public List<EstadoUsuario> ObtenerTodos()
        {
            try
            {
                // En este caso es una consulta directa, pero centralizarlo aquí 
                // nos permite añadir reglas de ordenamiento o filtrado de negocio en el futuro.
                return _repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error en CapaNegocio - EstadoUsuarioServicio: " + ex.Message, ex);
            }
        }
    }
}
