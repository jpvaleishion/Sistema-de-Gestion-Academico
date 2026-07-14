using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class PeriodoAcademicoServicio
    {
        private PeriodoAcademicoRepositorio repositorio = new PeriodoAcademicoRepositorio();

        // *cambio* - Instanciamos los servicios de permisos y bitácora
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        // *cambio* - Ahora recibe el ID del usuario logueado para seguridad y auditoría
        public void Guardar(PeriodoAcademico p, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Crear en frmPeriodosAcademicos
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar períodos académicos.");
            }

            // Validaciones de negocio existentes
            if (string.IsNullOrWhiteSpace(p.NombrePeriodo))
                throw new ArgumentException("El nombre del período es obligatorio.");

            if (p.FechaFin <= p.FechaInicio)
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");

            // Inserción en Base de Datos
            repositorio.Insertar(p);

            // *auditoria* - Registro detallado en bitácora incluyendo el rango de fechas
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Periodos Academicos",
                "Crear",
                $"Se registró el período académico: '{p.NombrePeriodo}' ({p.FechaInicio:dd/MM/yyyy} al {p.FechaFin:dd/MM/yyyy})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Actualizar(PeriodoAcademico p, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Modificar en frmPeriodosAcademicos
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar períodos académicos.");
            }

            // Validaciones de negocio existentes
            if (string.IsNullOrWhiteSpace(p.NombrePeriodo))
                throw new ArgumentException("El nombre del período es obligatorio.");

            if (p.FechaFin <= p.FechaInicio)
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");

            // Actualización en Base de Datos
            repositorio.Actualizar(p);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Periodos Academicos",
                "Modificar",
                $"Se actualizó el período académico ID {p.IdPeriodo} a: '{p.NombrePeriodo}' ({p.FechaInicio:dd/MM/yyyy} al {p.FechaFin:dd/MM/yyyy})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Eliminar(int idPeriodo, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Eliminar en frmPeriodosAcademicos
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar períodos académicos.");
            }

            if (idPeriodo <= 0)
                throw new ArgumentException("El identificador del período académico no es válido.");

            // Rescatamos el nombre del período antes de proceder a su borrado físico para la bitácora
            string nombrePeriodo = $"ID {idPeriodo}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idPeriodo);
                if (de_paso != null)
                {
                    nombrePeriodo = $"'{de_paso.NombrePeriodo}' (ID: {idPeriodo})";
                }
            }
            catch { /* Continuar silenciosamente si falla la lectura previa */ }

            // Eliminación en Base de Datos
            repositorio.Eliminar(idPeriodo);

            // *auditoria* - Registro en bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Periodos Academicos",
                "Eliminar",
                $"Se eliminó el período académico: {nombrePeriodo}."
            );
        }

        public List<PeriodoAcademico> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public PeriodoAcademico ObtenerPorId(int idPeriodo)
        {
            if (idPeriodo <= 0)
                throw new ArgumentException("El identificador del período académico no es válido.");

            return repositorio.ObtenerPorId(idPeriodo);
        }
    }
}