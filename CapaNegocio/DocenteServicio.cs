using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de docentes.
    /// Aplica reglas de negocio, validaciones y registra auditoría mediante bitácora y permisos.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class DocenteServicio
    {
        private readonly DocenteRepositorio repositorio = new DocenteRepositorio();
        private readonly PermisoServicio permisoService = new PermisoServicio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar un objeto <see cref="Docente"/> según reglas de negocio.
        /// </summary>
        /// <param name="d">Instancia de <see cref="Docente"/> a validar.</param>
        /// <exception cref="ArgumentNullException">Si el objeto docente es nulo.</exception>
        /// <exception cref="ArgumentException">Si alguna propiedad no cumple las restricciones de negocio.</exception>
        private void ValidarDocente(Docente d)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d), "El docente no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (d.Nombres.Length < 2 || d.Nombres.Length > 50)
                throw new ArgumentException("El nombre debe tener entre 2 y 50 caracteres.");

            // Permitir letras Unicode (acentos, ñ, etc.) y espacios
            if (!Regex.IsMatch(d.Nombres, @"^[\p{L}\s]+$"))
                throw new ArgumentException("El nombre solo puede contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (d.Apellidos.Length < 2 || d.Apellidos.Length > 50)
                throw new ArgumentException("Los apellidos deben tener entre 2 y 50 caracteres.");

            if (!Regex.IsMatch(d.Apellidos, @"^[\p{L}\s]+$"))
                throw new ArgumentException("Los apellidos solo pueden contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            if (d.Especialidad.Length < 3 || d.Especialidad.Length > 100)
                throw new ArgumentException("La especialidad debe tener entre 3 y 100 caracteres.");
        }

        /// <summary>
        /// Registra un error en la bitácora. No modifica la estructura de la bitácora existente.
        /// </summary>
        /// <param name="ex">Excepción capturada.</param>
        /// <param name="idUsuario">Identificador del usuario relacionado con la acción.</param>
        /// <param name="modulo">Módulo origen del error.</param>
        /// <param name="accion">Acción en la que ocurrió el error.</param>
        /// <param name="contexto">Contexto adicional para diagnóstico.</param>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacoraService.RegistrarAccion(idUsuario, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde el registro de errores para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra un nuevo docente aplicando validaciones de negocio y auditoría.
        /// </summary>
        /// <param name="d">Objeto <see cref="Docente"/> con los datos a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la operación.</param>
        /// <exception cref="InvalidOperationException">Si el usuario no tiene permiso o ocurre un error interno.</exception>
        public void Guardar(Docente d, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar docentes.");

                ValidarDocente(d);

                repositorio.Insertar(d);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Docentes",
                    "Crear",
                    $"Se registró al docente: {d.Nombres} {d.Apellidos} con especialidad en '{d.Especialidad}'."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Docentes",
                    "Crear",
                    $"Intentando guardar docente: Nombres='{(d != null ? d.Nombres : "null")}', Apellidos='{(d != null ? d.Apellidos : "null")}', Especialidad='{(d != null ? d.Especialidad : "null")}'"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar el docente.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un docente aplicando validaciones y auditoría.
        /// </summary>
        public void Actualizar(Docente d, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar docentes.");

                ValidarDocente(d);

                repositorio.Actualizar(d);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Docentes",
                    "Modificar",
                    $"Se actualizaron los datos del docente ID: {d.IdPersona} ({d.Nombres} {d.Apellidos}). Especialidad: '{d.Especialidad}'."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Docentes",
                    "Modificar",
                    $"Intentando actualizar docente ID={(d != null ? d.IdPersona.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar el docente.", ex);
            }
        }

        /// <summary>
        /// Elimina un docente del sistema y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idPersona, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Eliminar"))
                    throw new InvalidOperationException("No tiene permisos para eliminar docentes.");

                if (idPersona <= 0)
                    throw new ArgumentException("El identificador del docente no es válido.");

                string nombreDocente = $"ID {idPersona}";
                try
                {
                    var de_paso = repositorio.ObtenerPorId(idPersona);
                    if (de_paso != null)
                        nombreDocente = $"'{de_paso.Nombres} {de_paso.Apellidos}' (ID: {idPersona})";
                }
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Docentes", "Eliminar", $"Obteniendo datos para ID={idPersona}");
                }

                repositorio.Eliminar(idPersona);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Docentes",
                    "Eliminar",
                    $"Se eliminó al docente: {nombreDocente}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Docentes",
                    "Eliminar",
                    $"Intentando eliminar docente ID={idPersona}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al eliminar el docente.", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los docentes registrados.
        /// </summary>
        public List<Docente> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Docentes", "ObtenerTodos", "Obteniendo todos los docentes");
                throw new InvalidOperationException("Error al obtener los docentes.", ex);
            }
        }

        /// <summary>
        /// Obtiene un docente por su identificador.
        /// </summary>
        public Docente ObtenerPorId(int idPersona)
        {
            try
            {
                if (idPersona <= 0)
                    throw new ArgumentException("El identificador del docente no es válido.");

                return repositorio.ObtenerPorId(idPersona);
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Docentes", "ObtenerPorId", $"Obteniendo docente ID={idPersona}");
                throw new InvalidOperationException("Error al obtener el docente.", ex);
            }
        }
    }
}
