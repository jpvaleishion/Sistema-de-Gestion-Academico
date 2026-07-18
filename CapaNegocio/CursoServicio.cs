using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de cursos académicos.
    /// Aplica validaciones de negocio y registra auditoría mediante bitácora y permisos.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class CursoServicio
    {
        private readonly CursoRepositorio repositorio = new CursoRepositorio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();
        private readonly PermisoServicio permisoService = new PermisoServicio();

        /// <summary>
        /// Método auxiliar para validar curso.
        /// </summary>
        private void ValidarCurso(Curso c)
        {
            if (c == null)
                throw new ArgumentNullException(nameof(c), "El curso no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.NombreCurso.Length < 3 || c.NombreCurso.Length > 100)
                throw new ArgumentException("El nombre del curso debe tener entre 3 y 100 caracteres.");

            // Permitir letras Unicode (acentos, ñ, etc.) y espacios
            if (!Regex.IsMatch(c.NombreCurso, @"^[\p{L}\s]+$"))
                throw new ArgumentException("El nombre del curso solo puede contener letras y espacios.");

            if (c.Capacidad < 1 || c.Capacidad > 200)
                throw new ArgumentException("La capacidad del curso debe estar entre 1 y 200 alumnos.");
        }

        /// <summary>
        /// Registra un error en la bitácora. No modifica la estructura de la bitácora existente.
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
                // No propagar excepciones desde el registro de errores para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra un nuevo curso en el sistema aplicando validaciones y auditoría.
        /// </summary>
        public void Guardar(Curso c, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar cursos.");

                ValidarCurso(c);

                repositorio.Insertar(c);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Cursos",
                    "Crear",
                    $"Se registró el curso: '{c.NombreCurso}' con capacidad máxima de {c.Capacidad} alumnos."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Cursos",
                    "Crear",
                    $"Intentando guardar curso: Nombre='{(c != null ? c.NombreCurso : "null")}', Capacidad={(c != null ? c.Capacidad.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar el curso.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un curso existente aplicando validaciones y auditoría.
        /// </summary>
        public void Actualizar(Curso c, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar cursos.");

                ValidarCurso(c);

                repositorio.Actualizar(c);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Cursos",
                    "Modificar",
                    $"Se actualizó el curso ID {c.IdCurso} a: '{c.NombreCurso}' (Nueva capacidad: {c.Capacidad})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Cursos",
                    "Modificar",
                    $"Intentando actualizar curso ID={(c != null ? c.IdCurso.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar el curso.", ex);
            }
        }

        /// <summary>
        /// Elimina un curso del sistema y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idCurso, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Eliminar"))
                    throw new InvalidOperationException("No tiene permisos para eliminar cursos.");

                if (idCurso <= 0)
                    throw new ArgumentException("El identificador del curso no es válido.");

                string nombreCurso = $"ID {idCurso}";
                try
                {
                    var de_paso = repositorio.ObtenerPorId(idCurso);
                    if (de_paso != null) nombreCurso = $"'{de_paso.NombreCurso}' (ID: {idCurso})";
                }
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Cursos", "Eliminar", $"Obteniendo nombre para ID={idCurso}");
                }

                repositorio.Eliminar(idCurso);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Cursos",
                    "Eliminar",
                    $"Se eliminó el curso: {nombreCurso}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Cursos",
                    "Eliminar",
                    $"Intentando eliminar curso ID={idCurso}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al eliminar el curso.", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los cursos registrados.
        /// </summary>
        public List<Curso> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Cursos", "ObtenerTodos", "Obteniendo todos los cursos");
                throw new InvalidOperationException("Error al obtener los cursos.", ex);
            }
        }

        /// <summary>
        /// Obtiene un curso por su identificador.
        /// </summary>
        public Curso ObtenerPorId(int idCurso)
        {
            try
            {
                if (idCurso <= 0)
                    throw new ArgumentException("El identificador del curso no es válido.");

                return repositorio.ObtenerPorId(idCurso);
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Cursos", "ObtenerPorId", $"Obteniendo curso ID={idCurso}");
                throw new InvalidOperationException("Error al obtener el curso.", ex);
            }
        }
    }
}
