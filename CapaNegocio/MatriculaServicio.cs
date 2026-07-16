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
        /// Registra una nueva matrícula verificando asociaciones obligatorias y registrando auditoría.
        /// </summary>
        /// <param name="m">Entidad Matricula a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Guardar(Matricula m, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar matrículas.");
            }

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

            repositorio.Insertar(m);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Matriculas",
                "Crear",
                $"Se registró la matrícula: Matrícula ID {m.IdMatricula}, Estudiante ID {m.IdEstudiante}, Asignatura ID {m.IdAsignatura}, Docente ID {m.IdDocente}, Curso ID {m.IdCurso}, Período ID {m.IdPeriodo}."
            );
        }

        /// <summary>
        /// Actualiza una matrícula existente aplicando validaciones y registrando auditoría.
        /// </summary>
        /// <param name="m">Entidad Matricula a actualizar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Actualizar(Matricula m, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar matrículas.");
            }

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

            repositorio.Actualizar(m);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Matriculas",
                "Modificar",
                $"Se actualizó la Matrícula ID {m.IdMatricula}: Estudiante ID {m.IdEstudiante}, Asignatura ID {m.IdAsignatura}, Docente ID {m.IdDocente}, Curso ID {m.IdCurso}, Período ID {m.IdPeriodo}."
            );
        }

        /// <summary>
        /// Elimina una matrícula y registra la acción en la bitácora.
        /// </summary>
        /// <param name="idMatricula">Identificador de la matrícula a eliminar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Eliminar(int idMatricula, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar matrículas.");
            }

            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            string detalle = $"ID {idMatricula}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idMatricula);
                if (de_paso != null)
                {
                    detalle = $"ID {idMatricula} (Estudiante ID: {de_paso.IdEstudiante}, Asignatura ID: {de_paso.IdAsignatura})";
                }
            }
            catch { }

            repositorio.Eliminar(idMatricula);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Matriculas",
                "Eliminar",
                $"Se eliminó la matrícula: {detalle}."
            );
        }

        /// <summary>
        /// Obtiene todas las matrículas registradas.
        /// </summary>
        /// <returns>Lista de matrículas.</returns>
        public List<Matricula> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene una matrícula por su identificador.
        /// </summary>
        /// <param name="idMatricula">Identificador de la matrícula.</param>
        /// <returns>Entidad Matricula correspondiente.</returns>
        public Matricula ObtenerPorId(int idMatricula)
        {
            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            return repositorio.ObtenerPorId(idMatricula);
        }
    }
}
