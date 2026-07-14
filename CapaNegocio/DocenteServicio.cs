using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class DocenteServicio
    {
        private DocenteRepositorio repositorio = new DocenteRepositorio();

        // *cambio* - Instanciamos los servicios de permisos y bitácora
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        // RN-02: No se puede registrar un docente sin especialidad
        // *cambio* - Ahora recibe el ID del usuario logueado para validar seguridad y auditar
        public void Guardar(Docente d, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Crear en frmDocentes
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar docentes.");
            }

            // Validaciones de regla de negocio (RN-02)
            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            // Inserción en Base de Datos
            repositorio.Insertar(d);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Docentes",
                "Crear",
                $"Se registró al docente: {d.Nombres} {d.Apellidos} con especialidad en '{d.Especialidad}'."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Actualizar(Docente d, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Modificar en frmDocentes
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar docentes.");
            }

            // Validaciones de regla de negocio (RN-02)
            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            // Actualización en Base de Datos
            repositorio.Actualizar(d);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Docentes",
                "Modificar",
                $"Se actualizaron los datos del docente ID: {d.IdPersona} ({d.Nombres} {d.Apellidos}). Especialidad: '{d.Especialidad}'."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Eliminar(int idPersona, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Eliminar en frmDocentes
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar docentes.");
            }

            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            // Buscamos el nombre del docente antes de eliminarlo para registrarlo en la auditoría
            string nombreDocente = $"ID {idPersona}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idPersona);
                if (de_paso != null)
                {
                    nombreDocente = $"'{de_paso.Nombres} {de_paso.Apellidos}' (ID: {idPersona})";
                }
            }
            catch { /* Si falla la lectura, se mantiene el ID en el registro */ }

            // Eliminación en Base de Datos
            repositorio.Eliminar(idPersona);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Docentes",
                "Eliminar",
                $"Se eliminó al docente: {nombreDocente}."
            );
        }

        public List<Docente> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Docente ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            return repositorio.ObtenerPorId(idPersona);
        }
    }
}