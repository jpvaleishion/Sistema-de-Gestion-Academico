using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de docentes.
    /// Aplica reglas de negocio, validaciones y registra auditoría mediante bitácora y permisos.
    /// </summary>
    public class DocenteServicio
    {
        private DocenteRepositorio repositorio = new DocenteRepositorio();
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Registra un nuevo docente aplicando validaciones de negocio y auditoría.
        /// </summary>
        /// <param name="d">Entidad Docente a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Guardar(Docente d, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar docentes.");
            }

            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            repositorio.Insertar(d);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Docentes",
                "Crear",
                $"Se registró al docente: {d.Nombres} {d.Apellidos} con especialidad en '{d.Especialidad}'."
            );
        }

        /// <summary>
        /// Actualiza los datos de un docente aplicando validaciones y auditoría.
        /// </summary>
        /// <param name="d">Entidad Docente a actualizar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Actualizar(Docente d, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar docentes.");
            }

            if (string.IsNullOrWhiteSpace(d.Nombres))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(d.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(d.Especialidad))
                throw new ArgumentException("La especialidad es obligatoria.");

            repositorio.Actualizar(d);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Docentes",
                "Modificar",
                $"Se actualizaron los datos del docente ID: {d.IdPersona} ({d.Nombres} {d.Apellidos}). Especialidad: '{d.Especialidad}'."
            );
        }

        /// <summary>
        /// Elimina un docente del sistema y registra la acción en la bitácora.
        /// </summary>
        /// <param name="idPersona">Identificador de la persona/docente a eliminar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la acción.</param>
        public void Eliminar(int idPersona, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmDocentes", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar docentes.");
            }

            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            string nombreDocente = $"ID {idPersona}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idPersona);
                if (de_paso != null)
                {
                    nombreDocente = $"'{de_paso.Nombres} {de_paso.Apellidos}' (ID: {idPersona})";
                }
            }
            catch { }

            repositorio.Eliminar(idPersona);

            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Docentes",
                "Eliminar",
                $"Se eliminó al docente: {nombreDocente}."
            );
        }

        /// <summary>
        /// Obtiene todos los docentes registrados.
        /// </summary>
        /// <returns>Lista de docentes.</returns>
        public List<Docente> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un docente por su identificador.
        /// </summary>
        /// <param name="idPersona">Identificador del docente.</param>
        /// <returns>Entidad Docente correspondiente.</returns>
        public Docente ObtenerPorId(int idPersona)
        {
            if (idPersona <= 0)
                throw new ArgumentException("El identificador del docente no es válido.");

            return repositorio.ObtenerPorId(idPersona);
        }
    }
}
