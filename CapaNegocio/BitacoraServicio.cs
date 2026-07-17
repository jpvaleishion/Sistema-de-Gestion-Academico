using CapaDatos;
using CapaEntidades.Entidades;
using System;
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
        /// Registra una acción en la bitácora del sistema aplicando validaciones.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario que ejecuta la acción.</param>
        /// <param name="modulo">Módulo del sistema donde ocurre la acción.</param>
        /// <param name="accion">Acción realizada (Crear, Modificar, Eliminar, etc.).</param>
        /// <param name="descripcion">Descripción detallada de la acción.</param>
        public void RegistrarAccion(int idUsuario, string modulo, string accion, string descripcion)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.");

            if (string.IsNullOrWhiteSpace(modulo))
                throw new ArgumentException("El módulo es obligatorio.");

            if (string.IsNullOrWhiteSpace(accion))
                throw new ArgumentException("La acción es obligatoria.");

            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es obligatoria.");

            try
            {
                Bitacora b = new Bitacora
                {
                    IdUsuario = idUsuario,
                    Modulo = modulo.Trim(),
                    Accion = accion.Trim(),
                    Descripcion = descripcion.Trim(),
                    FechaHora = DateTime.Now
                };

                repositorio.Insertar(b);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones controlado
                throw new InvalidOperationException("Error al registrar la acción en la bitácora.", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los registros de bitácora almacenados.
        /// </summary>
        /// <returns>Lista de registros de bitácora.</returns>
        public List<Bitacora> ObtenerTodo()
        {
            try
            {
                return repositorio.ObtenerTodo();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener los registros de bitácora.", ex);
            }
        }
    }
}
