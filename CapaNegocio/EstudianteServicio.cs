using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de estudiantes.
    /// Aplica reglas de negocio, validaciones y registra auditoría mediante bitácora y permisos.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class EstudianteServicio
    {
        private readonly EstudianteRepositorio repositorio = new EstudianteRepositorio();
        private readonly PermisoServicio permisoService = new PermisoServicio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar estudiante.
        /// </summary>
        private void ValidarEstudiante(Estudiante e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "El estudiante no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");
            if (e.Nombres.Length < 2 || e.Nombres.Length > 50)
                throw new ArgumentException("El nombre debe tener entre 2 y 50 caracteres.");
            if (!Regex.IsMatch(e.Nombres, @"^[\p{L}\s]+$"))
                throw new ArgumentException("El nombre solo puede contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");
            if (e.Apellidos.Length < 2 || e.Apellidos.Length > 50)
                throw new ArgumentException("Los apellidos deben tener entre 2 y 50 caracteres.");
            if (!Regex.IsMatch(e.Apellidos, @"^[\p{L}\s]+$"))
                throw new ArgumentException("Los apellidos solo pueden contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");
            var regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regexEmail.IsMatch(e.Email))
                throw new ArgumentException("El email no tiene un formato válido.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");
            if (e.CodigoEstudiante.Length < 3 || e.CodigoEstudiante.Length > 20)
                throw new ArgumentException("El código estudiantil debe tener entre 3 y 20 caracteres.");
        }

        /// <summary>
        /// Registra un error en la bitácora. Usa idUsuario = 0 cuando no se dispone de usuario.
        /// </summary>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacoraService.RegistrarAccion(idUsuario, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde la bitácora para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra un nuevo estudiante aplicando validaciones y auditoría.
        /// </summary>
        public void Guardar(Estudiante e, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar estudiantes.");

                ValidarEstudiante(e);

                repositorio.Insertar(e);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Estudiantes",
                    "Crear",
                    $"Se registró al estudiante: {e.Nombres} {e.Apellidos} (Código: {e.CodigoEstudiante})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Estudiantes",
                    "Crear",
                    $"Intentando guardar estudiante: Nombres='{(e != null ? e.Nombres : "null")}', Apellidos='{(e != null ? e.Apellidos : "null")}', Código='{(e != null ? e.CodigoEstudiante : "null")}'"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar el estudiante.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un estudiante aplicando validaciones y auditoría.
        /// </summary>
        public void Actualizar(Estudiante e, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar estudiantes.");

                ValidarEstudiante(e);

                repositorio.Actualizar(e);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Estudiantes",
                    "Modificar",
                    $"Se actualizaron los datos del estudiante ID: {e.IdPersona} ({e.Nombres} {e.Apellidos}, Código: {e.CodigoEstudiante})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Estudiantes",
                    "Modificar",
                    $"Intentando actualizar estudiante ID={(e != null ? e.IdPersona.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar el estudiante.", ex);
            }
        }

        /// <summary>
        /// Elimina un estudiante del sistema y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idPersona, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Eliminar"))
                    throw new InvalidOperationException("No tiene permisos para eliminar estudiantes.");

                if (idPersona <= 0)
                    throw new ArgumentException("El identificador del estudiante no es válido.");

                string nombreEstudiante = $"ID {idPersona}";
                try
                {
                    var de_paso = repositorio.ObtenerPorId(idPersona);
                    if (de_paso != null)
                        nombreEstudiante = $"'{de_paso.Nombres} {de_paso.Apellidos}' (ID: {idPersona})";
                }
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Estudiantes", "Eliminar", $"Obteniendo datos para ID={idPersona}");
                }

                repositorio.Eliminar(idPersona);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Estudiantes",
                    "Eliminar",
                    $"Se eliminó al estudiante: {nombreEstudiante}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Estudiantes",
                    "Eliminar",
                    $"Intentando eliminar estudiante ID={idPersona}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al eliminar el estudiante.", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los estudiantes registrados.
        /// </summary>
        public List<Estudiante> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Estudiantes", "ObtenerTodos", "Obteniendo todos los estudiantes");
                throw new InvalidOperationException("Error al obtener los estudiantes.", ex);
            }
        }

        /// <summary>
        /// Obtiene un estudiante por su identificador.
        /// </summary>
        public Estudiante ObtenerPorId(int idPersona)
        {
            try
            {
                if (idPersona <= 0)
                    throw new ArgumentException("El identificador del estudiante no es válido.");

                var estudiante = repositorio.ObtenerPorId(idPersona);
                if (estudiante == null)
                {
                    var ex = new InvalidOperationException($"No existe un estudiante con ID {idPersona}.");
                    RegistrarErrorEnBitacora(ex, 0, "Estudiantes", "ObtenerPorId", $"ID no encontrado: {idPersona}");
                    throw ex;
                }

                return estudiante;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Estudiantes", "ObtenerPorId", $"Obteniendo estudiante ID={idPersona}");
                throw new InvalidOperationException("Error al obtener el estudiante.", ex);
            }
        }
    }
}
