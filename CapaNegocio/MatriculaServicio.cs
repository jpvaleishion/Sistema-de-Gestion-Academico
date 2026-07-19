using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de matrículas.
    /// Aplica reglas de negocio que aseguran la asociación correcta y registra auditoría.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class MatriculaServicio
    {
        private readonly MatriculaRepositorio repositorio = new MatriculaRepositorio();
        private readonly PermisoServicio permisoService = new PermisoServicio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar la entidad <see cref="Matricula"/> según las reglas de negocio.
        /// </summary>
        /// <param name="m">Instancia de <see cref="Matricula"/> a validar.</param>
        /// <exception cref="ArgumentNullException">Si la matrícula es nula.</exception>
        /// <exception cref="ArgumentException">Si alguna propiedad obligatoria no cumple las reglas de negocio.</exception>
        private void ValidarMatricula(Matricula m)
        {
            if (m == null)
                throw new ArgumentNullException(nameof(m), "La matrícula no puede ser nula.");

            if (m.IdEstudiante <= 0)
                throw new ArgumentException("Debe seleccionar un estudiante.");
            if (m.IdAsignatura <= 0)
                throw new ArgumentException("Debe seleccionar una asignatura.");
            if (m.IdDocente <= 0)
                throw new ArgumentException("Debe seleccionar un docente.");
            if (m.IdCurso <= 0)
                throw new ArgumentException("Debe seleccionar un curso.");
            if (m.IdPeriodo <= 0)
                throw new ArgumentException("Debe seleccionar un período académico.");
            if (m.FechaMatricula == DateTime.MinValue)
                throw new ArgumentException("La fecha de inscripción no es válida.");
            if (string.IsNullOrWhiteSpace(m.Estado))
                throw new ArgumentException("El estado de la matrícula es obligatorio.");
        }

        /// <summary>
        /// Registra un error en la bitácora. Intenta almacenar detalles mínimos del error sin alterar la estructura actual de la bitácora.
        /// </summary>
        /// <remarks>
        /// No se propagan excepciones desde este método para evitar que fallos en la auditoría cancelen la operación principal.
        /// </remarks>
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
        /// Registra una nueva matrícula verificando asociaciones obligatorias y registrando auditoría.
        /// </summary>
        public void Guardar(Matricula m, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar matrículas.");

                ValidarMatricula(m);

                repositorio.Insertar(m);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Matriculas",
                    "Crear",
                    $"Se registró la matrícula ID {m.IdMatricula}, Estudiante ID {m.IdEstudiante}, Asignatura ID {m.IdAsignatura}, Docente ID {m.IdDocente}, Curso ID {m.IdCurso}, Período ID {m.IdPeriodo}, Estado: {m.Estado}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Matriculas",
                    "Crear",
                    $"Intentando guardar matrícula: EstudianteID={(m != null ? m.IdEstudiante.ToString() : "null")}, AsignaturaID={(m != null ? m.IdAsignatura.ToString() : "null")}, CursoID={(m != null ? m.IdCurso.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar la matrícula.", ex);
            }
        }

        /// <summary>
        /// Actualiza una matrícula existente aplicando validaciones y registrando auditoría.
        /// </summary>
        public void Actualizar(Matricula m, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar matrículas.");

                ValidarMatricula(m);

                repositorio.Actualizar(m);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Matriculas",
                    "Modificar",
                    $"Se actualizó la Matrícula ID {m.IdMatricula}: Estudiante ID {m.IdEstudiante}, Asignatura ID {m.IdAsignatura}, Docente ID {m.IdDocente}, Curso ID {m.IdCurso}, Período ID {m.IdPeriodo}, Estado: {m.Estado}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Matriculas",
                    "Modificar",
                    $"Intentando actualizar matrícula ID={(m != null ? m.IdMatricula.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar la matrícula.", ex);
            }
        }

        /// <summary>
        /// Elimina una matrícula y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idMatricula, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Eliminar"))
                    throw new InvalidOperationException("No tiene permisos para eliminar matrículas.");

                if (idMatricula <= 0)
                    throw new ArgumentException("El identificador de la matrícula no es válido.");

                string detalle = $"ID {idMatricula}";
                try
                {
                    var de_paso = repositorio.ObtenerPorId(idMatricula);
                    if (de_paso != null)
                        detalle = $"ID {idMatricula} (Estudiante ID: {de_paso.IdEstudiante}, Asignatura ID: {de_paso.IdAsignatura}, Estado: {de_paso.Estado})";
                }
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Matriculas", "Eliminar", $"Obteniendo detalle para ID={idMatricula}");
                }

                repositorio.Eliminar(idMatricula);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Matriculas",
                    "Eliminar",
                    $"Se eliminó la matrícula: {detalle}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Matriculas",
                    "Eliminar",
                    $"Intentando eliminar matrícula ID={idMatricula}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al eliminar la matrícula.", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las matrículas registradas.
        /// </summary>
        /// <returns>Lista de <see cref="Matricula"/> con las matrículas registradas.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error al acceder al repositorio.</exception>
        public List<Matricula> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Matriculas", "ObtenerTodos", "Obteniendo todas las matrículas");
                throw new InvalidOperationException("Error al obtener las matrículas.", ex);
            }
        }

        /// <summary>
        /// Obtiene una matrícula por su identificador.
        /// </summary>
        /// <param name="idMatricula">Identificador de la matrícula a obtener.</param>
        /// <returns>Instancia de <see cref="Matricula"/> si se encuentra; de lo contrario lanza una excepción.</returns>
        /// <exception cref="ArgumentException">Si el identificador proporcionado no es válido.</exception>
        /// <exception cref="InvalidOperationException">Si no existe la matrícula o ocurre un error al acceder al repositorio.</exception>
        public Matricula ObtenerPorId(int idMatricula)
        {
            try
            {
                if (idMatricula <= 0)
                    throw new ArgumentException("El identificador de la matrícula no es válido.");

                var matricula = repositorio.ObtenerPorId(idMatricula);
                if (matricula == null)
                {
                    var ex = new InvalidOperationException($"No existe una matrícula con ID {idMatricula}.");
                    RegistrarErrorEnBitacora(ex, 0, "Matriculas", "ObtenerPorId", $"ID no encontrado: {idMatricula}");
                    throw ex;
                }

                return matricula;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Matriculas", "ObtenerPorId", $"Obteniendo matrícula ID={idMatricula}");
                throw new InvalidOperationException("Error al obtener la matrícula.", ex);
            }
        }
    }
}
