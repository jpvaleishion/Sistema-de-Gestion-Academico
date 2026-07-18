using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de roles.
    /// Proporciona operaciones de lectura sobre los roles almacenados.
    /// Registra errores en la bitácora sin cambiar las firmas públicas.
    /// </summary>
    public class RolServicio
    {
        private readonly RolRepositorio repositorio = new RolRepositorio();
        private readonly BitacoraServicio bitacora = new BitacoraServicio();

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

        /// <summary>
        /// Obtiene la lista completa de roles procesada desde el repositorio.
        /// </summary>
        public List<Rol> ObtenerTodos()
        {
            try
            {
                var roles = repositorio.ObtenerTodos();
                if (roles == null || roles.Count == 0)
                {
                    var ex = new InvalidOperationException("No se encontraron roles registrados en el sistema.");
                    RegistrarErrorEnBitacora(ex, 0, "Roles", "ObtenerTodos", "Lista vacía o nula desde repositorio");
                    throw ex;
                }

                return roles;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Roles", "ObtenerTodos", "Obteniendo todos los roles");
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
            {
                var argEx = new ArgumentException("El identificador del rol no es válido.", nameof(idRol));
                RegistrarErrorEnBitacora(argEx, 0, "Roles", "ObtenerPorId", $"Id inválido: {idRol}");
                throw argEx;
            }

            try
            {
                var rol = repositorio.ObtenerPorId(idRol);
                if (rol == null)
                {
                    var ex = new InvalidOperationException($"No existe un rol con ID {idRol}.");
                    RegistrarErrorEnBitacora(ex, 0, "Roles", "ObtenerPorId", $"ID no encontrado: {idRol}");
                    throw ex;
                }

                return rol;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Roles", "ObtenerPorId", $"Obteniendo rol ID={idRol}");
                throw new InvalidOperationException("Error al obtener el rol.", ex);
            }
        }
    }
}
