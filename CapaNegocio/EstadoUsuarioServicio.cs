using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

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
                var lista = _repositorio.ObtenerTodos();

                if (lista == null || lista.Count == 0)
                    throw new InvalidOperationException("No se encontraron estados de usuario registrados.");

                return lista;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener los estados de usuario.", ex);
            }
        }

        /// <summary>
        /// Obtiene un estado de usuario por su identificador.
        /// </summary>
        /// <param name="idEstado">Identificador del estado de usuario.</param>
        /// <returns>Entidad EstadoUsuario correspondiente.</returns>
        public EstadoUsuario ObtenerPorId(int idEstado)
        {
            if (idEstado <= 0)
                throw new ArgumentException("El identificador del estado de usuario no es válido.");

            try
            {
                var estado = _repositorio.ObtenerPorId(idEstado);
                if (estado == null)
                    throw new InvalidOperationException($"No existe un estado de usuario con ID {idEstado}.");

                return estado;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el estado de usuario.", ex);
            }
        }
    }
}
