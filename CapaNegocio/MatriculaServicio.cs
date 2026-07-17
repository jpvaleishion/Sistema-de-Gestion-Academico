using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de matrículas.
    /// Aplica reglas de negocio que aseguran la asociación correcta y registra auditoría.
    /// </summary>
    public class MatriculaServicio
    {
        private MatriculaRepositorio repositorio = new MatriculaRepositorio();
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar matrícula.
        /// </summary>
        private void ValidarMatricula(Matricula m)
        {
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
        /// Registra una nueva matrícula verificando asociaciones obligatorias y registrando auditoría.
        /// </summary>
        public void Guardar(Matricula m, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar matrículas.");

            ValidarMatricula(m);

            try
            {
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
                throw new InvalidOperationException("Error al guardar la matrícula.", ex);
            }
        }

        /// <summary>
        /// Actualiza una matrícula existente aplicando validaciones y registrando auditoría.
        /// </summary>
        public void Actualizar(Matricula m, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar matrículas.");

            ValidarMatricula(m);

            try
            {
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
                throw new InvalidOperationException("Error al actualizar la matrícula.", ex);
            }
        }

        /// <summary>
        /// Elimina una matrícula y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idMatricula, int idUsuarioLogueado)
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
            catch { }

            try
            {
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
                throw new InvalidOperationException("Error al eliminar la matrícula.", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las matrículas registradas.
        /// </summary>
        public List<Matricula> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener las matrículas.", ex);
            }
        }

        /// <summary>
        /// Obtiene una matrícula por su identificador.
        /// </summary>
        public Matricula ObtenerPorId(int idMatricula)
        {
            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            try
            {
                var matricula = repositorio.ObtenerPorId(idMatricula);
                if (matricula == null)
                    throw new InvalidOperationException($"No existe una matrícula con ID {idMatricula}.");

                return matricula;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener la matrícula.", ex);
            }
        }
    }
}
