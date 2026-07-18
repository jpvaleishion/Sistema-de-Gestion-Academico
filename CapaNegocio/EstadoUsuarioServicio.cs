using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para el catálogo de estados de usuario.
    /// Ahora registra errores en la bitácora sin cambiar las firmas públicas.
    /// </summary>
    public class EstadoUsuarioServicio
    {
        private readonly EstadoUsuarioRepositorio _repositorio = new EstadoUsuarioRepositorio();
        private readonly BitacoraServicio _bitacora = new BitacoraServicio();

        /// <summary>
        /// Registra un error en la bitácora. Usa idUsuario = 0 para acciones del sistema.
        /// </summary>
        private void RegistrarErrorEnBitacora(Exception ex, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                _bitacora.RegistrarAccion(0, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde la bitácora para no afectar la lógica de negocio.
            }
        }

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
                {
                    var ex = new InvalidOperationException("No se encontraron estados de usuario registrados.");
                    RegistrarErrorEnBitacora(ex, "EstadoUsuario", "ObtenerTodos", "Lista vacía o nula desde repositorio");
                    throw ex;
                }

                return lista;
            }
            catch (Exception ex)
            {
                // Registrar en bitácora y envolver la excepción para mantener comportamiento consistente.
                RegistrarErrorEnBitacora(ex, "EstadoUsuario", "ObtenerTodos", "Obteniendo todos los estados de usuario");
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
            {
                var argEx = new ArgumentException("El identificador del estado de usuario no es válido.", nameof(idEstado));
                RegistrarErrorEnBitacora(argEx, "EstadoUsuario", "ObtenerPorId", $"Id inválido: {idEstado}");
                throw argEx;
            }

            try
            {
                var estado = _repositorio.ObtenerPorId(idEstado);
                if (estado == null)
                {
                    var ex = new InvalidOperationException($"No existe un estado de usuario con ID {idEstado}.");
                    RegistrarErrorEnBitacora(ex, "EstadoUsuario", "ObtenerPorId", $"ID no encontrado: {idEstado}");
                    throw ex;
                }

                return estado;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, "EstadoUsuario", "ObtenerPorId", $"Obteniendo estado ID={idEstado}");
                throw new InvalidOperationException("Error al obtener el estado de usuario.", ex);
            }
        }
    }
}
