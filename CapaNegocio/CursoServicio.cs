using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

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
        /// Registra un nuevo curso en el sistema aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="c">Entidad Curso a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Guardar(Curso c, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar cursos.");
            }

            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.Capacidad < 1)
                throw new ArgumentException("La capacidad del curso debe ser mayor a 0.");

            repositorio.Insertar(c);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Cursos",
                "Crear",
                $"Se registró el curso: '{c.NombreCurso}' con una capacidad máxima de {c.Capacidad} alumnos."
            );
        }

        /// <summary>
        /// Actualiza los datos de un curso existente aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="c">Entidad Curso a actualizar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Actualizar(Curso c, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar cursos.");
            }

            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.Capacidad < 1)
                throw new ArgumentException("La capacidad del curso debe ser mayor a 0.");

            repositorio.Actualizar(c);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Cursos",
                "Modificar",
                $"Se actualizó el curso ID {c.IdCurso} a: '{c.NombreCurso}' (Nueva capacidad: {c.Capacidad})."
            );
        }

        /// <summary>
        /// Elimina un curso del sistema y registra la acción en la bitácora.
        /// </summary>
        /// <param name="idCurso">Identificador del curso a eliminar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Eliminar(int idCurso, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar cursos.");
            }

            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            string nombreCurso = $"ID {idCurso}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idCurso);
                if (de_paso != null) nombreCurso = $"'{de_paso.NombreCurso}' (ID: {idCurso})";
            }
            catch { }

            repositorio.Eliminar(idCurso);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Cursos",
                "Eliminar",
                $"Se eliminó el curso: {nombreCurso}."
            );
        }

        /// <summary>
        /// Obtiene todos los cursos registrados.
        /// </summary>
        /// <returns>Lista de cursos.</returns>
        public List<Curso> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un curso por su identificador.
        /// </summary>
        /// <param name="idCurso">Identificador del curso.</param>
        /// <returns>Entidad Curso correspondiente.</returns>
        public Curso ObtenerPorId(int idCurso)
        {
            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            return repositorio.ObtenerPorId(idCurso);
        }
    }
}
