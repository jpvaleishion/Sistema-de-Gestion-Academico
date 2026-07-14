using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class EstudianteServicio
    {
        private EstudianteRepositorio repositorio = new EstudianteRepositorio();

        // *cambio* - Instanciamos los servicios de seguridad y auditoría
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        // RN-01: Nombres, Apellidos, Email y CodigoEstudiante son obligatorios
        // *cambio* - Ahora recibe el ID del usuario logueado para seguridad y bitácora
        public void Guardar(Estudiante e, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Crear en frmEstudiantes
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar estudiantes.");
            }

            // Validaciones de regla de negocio existentes (RN-01)
            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");

            // Inserción en Base de Datos
            repositorio.Insertar(e);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Estudiantes",
                "Crear",
                $"Se registró al estudiante: {e.Nombres} {e.Apellidos} (Código: {e.CodigoEstudiante})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Actualizar(Estudiante e, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Modificar en frmEstudiantes
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar estudiantes.");
            }

            // Validaciones de regla de negocio existentes (RN-01)
            if (string.IsNullOrWhiteSpace(e.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(e.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.CodigoEstudiante))
                throw new ArgumentException("El código estudiantil es obligatorio.");

            // Actualización en Base de Datos
            repositorio.Actualizar(e);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Estudiantes",
                "Modificar",
                $"Se actualizaron los datos del estudiante ID: {e.IdPersona} ({e.Nombres} {e.Apellidos}, Código: {e.CodigoEstudiante})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Eliminar(int idPersona, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Eliminar en frmEstudiantes
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar estudiantes.");
            }

            if (idPersona <= 0)
                throw new ArgumentException("El identificador del estudiante no es válido.");

            // Intentamos obtener el nombre del estudiante antes de borrar para que la bitácora quede impecable
            string nombreEstudiante = $"ID {idPersona}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idPersona);
                if (de_paso != null)
                {
                    nombreEstudiante = $"'{de_paso.Nombres} {de_paso.Apellidos}' (ID: {idPersona})";
                }
            }
            catch { /* Si falla la consulta previa, se mantiene el ID base */ }

            // Eliminación en Base de Datos
            repositorio.Eliminar(idPersona);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Estudiantes",
                "Eliminar",
                $"Se eliminó al estudiante: {nombreEstudiante}."
            );
        }

        public List<Estudiante> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Estudiante ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del estudiante no es válido.");

            return repositorio.ObtenerPorId(idPersona);
        }
    }
}