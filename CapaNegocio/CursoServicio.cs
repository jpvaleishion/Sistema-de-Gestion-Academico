using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CursoServicio
    {
        private CursoRepositorio repositorio = new CursoRepositorio();

        // *cambio* - Instanciamos los servicios de permisos y bitácora
        private BitacoraServicio bitacoraService = new BitacoraServicio();
        private PermisoServicio permisoService = new PermisoServicio();

        // *cambio* - Ahora recibe el ID del usuario logueado para seguridad y auditoría
        public void Guardar(Curso c, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Crear en frmCursos
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar cursos.");
            }

            // Validaciones de negocio existentes
            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.Capacidad < 1)
                throw new ArgumentException("La capacidad del curso debe ser mayor a 0.");

            // Inserción en Base de Datos
            repositorio.Insertar(c);

            // *auditoria* - Registro en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Cursos",
                "Crear",
                $"Se registró el curso: '{c.NombreCurso}' con una capacidad máxima de {c.Capacidad} alumnos."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Actualizar(Curso c, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Modificar en frmCursos
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar cursos.");
            }

            // Validaciones de negocio existentes
            if (string.IsNullOrWhiteSpace(c.NombreCurso))
                throw new ArgumentException("El nombre del curso es obligatorio.");

            if (c.Capacidad < 1)
                throw new ArgumentException("La capacidad del curso debe ser mayor a 0.");

            // Actualización en Base de Datos
            repositorio.Actualizar(c);

            // *auditoria* - Registro en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Cursos",
                "Modificar",
                $"Se actualizó el curso ID {c.IdCurso} a: '{c.NombreCurso}' (Nueva capacidad: {c.Capacidad})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Eliminar(int idCurso, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Eliminar en frmCursos
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCursos", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar cursos.");
            }

            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            // Intentamos obtener el nombre del curso antes de borrar para que la bitácora no sea tan fría
            string nombreCurso = $"ID {idCurso}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idCurso);
                if (de_paso != null) nombreCurso = $"'{de_paso.NombreCurso}' (ID: {idCurso})";
            }
            catch { /* Continuar si falla la lectura preliminar */ }

            // Eliminación en Base de Datos
            repositorio.Eliminar(idCurso);

            // *auditoria* - Registro en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Cursos",
                "Eliminar",
                $"Se eliminó el curso: {nombreCurso}."
            );
        }

        // Los métodos de lectura no requieren validación de permisos de edición ni bitácora de cambios
        public List<Curso> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Curso ObtenerPorId(int idCurso)
        {
            if (idCurso <= 0)
                throw new ArgumentException("El identificador del curso no es válido.");

            return repositorio.ObtenerPorId(idCurso);
        }
    }
}