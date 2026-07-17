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
    /// </summary>
    public class DocenteServicio
    {
        private DocenteRepositorio repositorio = new DocenteRepositorio();
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar docente.
        /// </summary>
        private void ValidarDocente(Docente d)
        {
            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (d.Nombres.Length < 2 || d.Nombres.Length > 50)
                throw new ArgumentException("El nombre debe tener entre 2 y 50 caracteres.");

            if (!Regex.IsMatch(d.Nombres, @"^[a-zA-Z\s]+$"))
                throw new ArgumentException("El nombre solo puede contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (d.Apellidos.Length < 2 || d.Apellidos.Length > 50)
                throw new ArgumentException("Los apellidos deben tener entre 2 y 50 caracteres.");

            if (!Regex.IsMatch(d.Apellidos, @"^[a-zA-Z\s]+$"))
                throw new ArgumentException("Los apellidos solo pueden contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            if (d.Especialidad.Length < 3 || d.Especialidad.Length > 100)
                throw new ArgumentException("La especialidad debe tener entre 3 y 100 caracteres.");
        }

        /// <summary>
        /// Registra un nuevo docente aplicando validaciones de negocio y auditoría.
        /// </summary>
        public void Guardar(Docente d, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar docentes.");

            ValidarDocente(d);

            try
            {
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
                throw new InvalidOperationException("Error al guardar el docente.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un docente aplicando validaciones y auditoría.
        /// </summary>
        public void Actualizar(Docente d, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar docentes.");

            ValidarDocente(d);

            try
            {
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
                throw new InvalidOperationException("Error al actualizar el docente.", ex);
            }
        }

        /// <summary>
        /// Elimina un docente del sistema y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idPersona, int idUsuarioLogueado)
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
            catch { }

            try
            {
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
                throw new InvalidOperationException("Error al obtener los docentes.", ex);
            }
        }

        /// <summary>
        /// Obtiene un docente por su identificador.
        /// </summary>
        public Docente ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            try
            {
                return repositorio.ObtenerPorId(idPersona);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el docente.", ex);
            }
        }
    }
}
