using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class MatriculaServicio
    {
        private MatriculaRepositorio repositorio = new MatriculaRepositorio();

        // *cambio* - Instanciamos los servicios de permisos y bitácora
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        // RN-05: La matrícula debe estar asociada a Estudiante, Asignatura, Docente, Curso y Período
        // *cambio* - Ahora recibe el ID del usuario logueado para control de acceso y auditoría
        public void Guardar(Matricula m, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Crear en frmMatriculas
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar matrículas.");
            }

            // Validaciones de regla de negocio (RN-05)
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

            // Inserción en Base de Datos
            repositorio.Insertar(m);

            // *auditoria* - Registro detallado en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Matriculas",
                "Crear",
                $"Se registró matrícula para el Estudiante ID {m.IdEstudiante} en la Asignatura ID {m.IdAsignatura} (Docente ID: {m.IdDocente}, Curso ID: {m.IdCurso}, Período ID: {m.IdPeriodo})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Actualizar(Matricula m, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Modificar en frmMatriculas
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar matrículas.");
            }

            // Validaciones de regla de negocio (RN-05)
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

            // Actualización en Base de Datos
            repositorio.Actualizar(m);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Matriculas",
                "Modificar",
                $"Se modificó la Matrícula ID {m.IdMatricula}. Nuevos datos -> Estudiante ID: {m.IdEstudiante}, Asignatura ID: {m.IdAsignatura}, Docente ID: {m.IdDocente}, Curso ID: {m.IdCurso}, Período ID: {m.IdPeriodo}."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Eliminar(int idMatricula, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Eliminar en frmMatriculas
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar matrículas.");
            }

            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            // Rescatamos datos clave de la matrícula antes de proceder a borrarla físicamente
            string detalleMatricula = $"ID {idMatricula}";
            try
            {
                var matriculaPrev = repositorio.ObtenerPorId(idMatricula);
                if (matriculaPrev != null)
                {
                    detalleMatricula = $"ID {idMatricula} (Estudiante ID: {matriculaPrev.IdEstudiante}, Asignatura ID: {matriculaPrev.IdAsignatura})";
                }
            }
            catch { /* Continuar silenciosamente si falla la lectura preliminar */ }

            // Eliminación en Base de Datos
            repositorio.Eliminar(idMatricula);

            // *auditoria* - Registro en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Matriculas",
                "Eliminar",
                $"Se eliminó la matrícula: {detalleMatricula}."
            );
        }

        public List<Matricula> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Matricula ObtenerPorId(int idMatricula)
        {
            if (idMatricula <= 0)
                throw new ArgumentException("El identificador de la matrícula no es válido.");

            return repositorio.ObtenerPorId(idMatricula);
        }
    }
}