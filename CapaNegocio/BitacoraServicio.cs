using CapaDatos;
using CapaEntidades.Entidades;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de registros de bitácora.
    /// Permite registrar acciones de usuario y recuperar el historial completo.
    /// </summary>
    public class BitacoraServicio
    {
        private BitacoraRepositorio repositorio = new BitacoraRepositorio();

        /// <summary>
        /// Registra una acción en la bitácora del sistema.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario que ejecuta la acción.</param>
        /// <param name="modulo">Módulo del sistema donde ocurre la acción.</param>
        /// <param name="accion">Acción realizada (Crear, Modificar, Eliminar, etc.).</param>
        /// <param name="descripcion">Descripción detallada de la acción.</param>
        public void RegistrarAccion(int idUsuario, string modulo, string accion, string descripcion)
        {
            Bitacora b = new Bitacora
            {
                IdUsuario = idUsuario,
                Modulo = modulo,
                Accion = accion,
                Descripcion = descripcion
            };

            repositorio.Insertar(b);
        }

        /// <summary>
        /// Obtiene todos los registros de bitácora almacenados.
        /// </summary>
        /// <returns>Lista de registros de bitácora.</returns>
        public List<Bitacora> ObtenerTodo()
        {
            return repositorio.ObtenerTodo();
        }
    }
}
