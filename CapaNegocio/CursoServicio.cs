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
    /// </summary>
    public class CursoServicio
    {
        private CursoRepositorio repositorio = new CursoRepositorio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();
        private PermisoServicio permisoService = new PermisoServicio();

        /// <summary>
        /// Método auxiliar para validar curso.
        /// </summary>
        private void ValidarCurso(Curso c)
        {
            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.NombreCurso.Length < 3 || c.NombreCurso.Length > 100)
                throw new ArgumentException("El nombre del curso debe tener entre 3 y 100 caracteres.");

            if (!Regex.IsMatch(c.NombreCurso, @"^[a-zA-Z\s]+$"))
                throw new ArgumentException("El nombre del curso solo puede contener letras y espacios.");

            if (c.Capacidad < 1 || c.Capacidad > 200)
                throw new ArgumentException("La capacidad del curso debe estar entre 1 y 200 alumnos.");
        }

        /// <summary>
        /// Registra un nuevo curso en el sistema aplicando validaciones y auditoría.
        /// </summary>
        public void Guardar(Curso c, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar cursos.");

            ValidarCurso(c);

            try
            {
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
                throw new InvalidOperationException("Error al guardar el curso.", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un curso existente aplicando validaciones y auditoría.
        /// </summary>
        public void Actualizar(Curso c, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar cursos.");

            ValidarCurso(c);

            try
            {
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
                throw new InvalidOperationException("Error al actualizar el curso.", ex);
            }
        }

        /// <summary>
        /// Elimina un curso del sistema y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idCurso, int idUsuarioLogueado)
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
            catch { }

            try
            {
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
                throw new InvalidOperationException("Error al obtener los cursos.", ex);
            }
        }

        /// <summary>
        /// Obtiene un curso por su identificador.
        /// </summary>
        public Curso ObtenerPorId(int idCurso)
        {
            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            try
            {
                return repositorio.ObtenerPorId(idCurso);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el curso.", ex);
            }
        }
    }
}
