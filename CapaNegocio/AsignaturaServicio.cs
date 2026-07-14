using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class AsignaturaServicio
    {
        private AsignaturaRepositorio repositorio = new AsignaturaRepositorio();

        // *cambio* - Instanciamos los servicios de bitácora y permisos
        private BitacoraServicio bitacoraService = new BitacoraServicio();
        private PermisoServicio permisoService = new PermisoServicio();

        // RN-03: Una asignatura debe tener al menos 1 crédito
        // *cambio* - Ahora recibe el ID del usuario logueado para verificar permisos y auditar
        public void Guardar(Asignatura a, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el usuario tiene permiso para Crear en este formulario
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar asignaturas.");
            }

            // Validaciones de negocio existentes
            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Creditos < 1)
                throw new ArgumentException("La asignatura debe tener al menos 1 crédito.");

            // Inserción en la Base de Datos
            repositorio.Insertar(a);

            // *auditoria* - Se registra la creación exitosa en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Asignaturas",
                "Crear",
                $"Se registró la asignatura: '{a.Nombre}' con {a.Creditos} créditos."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado para verificar permisos y auditar
        public void Actualizar(Asignatura a, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el usuario tiene permiso para Modificar en este formulario
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar asignaturas.");
            }

            // Validaciones de negocio existentes
            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la asignatura es obligatorio.");

            if (a.Creditos < 1)
                throw new ArgumentException("La asignatura debe tener al menos 1 crédito.");

            // Actualización en la Base de Datos
            repositorio.Actualizar(a);

            // *auditoria* - Se registra la modificación en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Asignaturas",
                "Modificar",
                $"Se actualizó la asignatura ID {a.IdAsignatura} a: '{a.Nombre}' ({a.Creditos} créditos)."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado para verificar permisos y auditar
        public void Eliminar(int idAsignatura, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el usuario tiene permiso para Eliminar en este formulario
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar asignaturas.");
            }

            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            // Opcional: Obtener el nombre antes de borrar para un registro de bitácora más detallado
            string nombreAsignatura = "ID " + idAsignatura;
            try
            {
                var de_paso = repositorio.ObtenerPorId(idAsignatura);
                if (de_paso != null) nombreAsignatura = $"'{de_paso.Nombre}' (ID: {idAsignatura})";
            }
            catch { /* Continuar si falla la lectura previa */ }

            // Eliminación en la Base de Datos
            repositorio.Eliminar(idAsignatura);

            // *auditoria* - Se registra la eliminación en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Asignaturas",
                "Eliminar",
                $"Se eliminó la asignatura: {nombreAsignatura}."
            );
        }

        // Los métodos de lectura no requieren validación de permisos de edición ni bitácora de cambios
        public List<Asignatura> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Asignatura ObtenerPorId(int idAsignatura)
        {
            if (idAsignatura <= 0)
                throw new ArgumentException("El identificador de la asignatura no es válido.");

            return repositorio.ObtenerPorId(idAsignatura);
        }
    }
}