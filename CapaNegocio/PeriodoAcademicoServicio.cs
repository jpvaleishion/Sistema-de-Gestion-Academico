using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de períodos académicos.
    /// Aplica validaciones de integridad, controla permisos y registra auditoría.
    /// </summary>
    public class PeriodoAcademicoServicio
    {
        private PeriodoAcademicoRepositorio repositorio = new PeriodoAcademicoRepositorio();
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar período académico.
        /// </summary>
        private void ValidarPeriodo(PeriodoAcademico p)
        {
            if (string.IsNullOrWhiteSpace(p.NombrePeriodo))
                throw new ArgumentException("El nombre del período es obligatorio.");

            if (p.NombrePeriodo.Length < 3 || p.NombrePeriodo.Length > 100)
                throw new ArgumentException("El nombre del período debe tener entre 3 y 100 caracteres.");

            if (p.FechaInicio == DateTime.MinValue)
                throw new ArgumentException("La fecha de inicio no es válida.");

            if (p.FechaFin == DateTime.MinValue)
                throw new ArgumentException("La fecha de fin no es válida.");

            if (p.FechaFin <= p.FechaInicio)
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        /// <summary>
        /// Registra un nuevo período académico verificando permisos, validaciones y registrando auditoría.
        /// </summary>
        public void Guardar(PeriodoAcademico p, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar períodos académicos.");

            ValidarPeriodo(p);

            try
            {
                repositorio.Insertar(p);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Periodos Academicos",
                    "Crear",
                    $"Se registró el período académico: '{p.NombrePeriodo}' ({p.FechaInicio:dd/MM/yyyy} al {p.FechaFin:dd/MM/yyyy})."
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al guardar el período académico.", ex);
            }
        }

        /// <summary>
        /// Actualiza un período académico existente aplicando validaciones, control de permisos y auditoría.
        /// </summary>
        public void Actualizar(PeriodoAcademico p, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar períodos académicos.");

            ValidarPeriodo(p);

            try
            {
                repositorio.Actualizar(p);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Periodos Academicos",
                    "Modificar",
                    $"Se actualizó el período académico ID {p.IdPeriodo} a: '{p.NombrePeriodo}' ({p.FechaInicio:dd/MM/yyyy} al {p.FechaFin:dd/MM/yyyy})."
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al actualizar el período académico.", ex);
            }
        }

        /// <summary>
        /// Elimina un período académico tras verificar permisos y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idPeriodo, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Eliminar"))
                throw new InvalidOperationException("No tiene permisos para eliminar períodos académicos.");

            if (idPeriodo <= 0)
                throw new ArgumentException("El identificador del período académico no es válido.");

            string nombrePeriodo = $"ID {idPeriodo}";
            try
            {
                var de_paso = repositorio.ObtenerPorId(idPeriodo);
                if (de_paso != null)
                    nombrePeriodo = $"'{de_paso.NombrePeriodo}' (ID: {idPeriodo})";
            }
            catch { }

            try
            {
                repositorio.Eliminar(idPeriodo);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Periodos Academicos",
                    "Eliminar",
                    $"Se eliminó el período académico: {nombrePeriodo}."
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al eliminar el período académico.", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los períodos académicos registrados.
        /// </summary>
        public List<PeriodoAcademico> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener los períodos académicos.", ex);
            }
        }

        /// <summary>
        /// Obtiene un período académico por su identificador.
        /// </summary>
        public PeriodoAcademico ObtenerPorId(int idPeriodo)
        {
            if (idPeriodo <= 0)
                throw new ArgumentException("El identificador del período académico no es válido.");

            try
            {
                var periodo = repositorio.ObtenerPorId(idPeriodo);
                if (periodo == null)
                    throw new InvalidOperationException($"No existe un período académico con ID {idPeriodo}.");

                return periodo;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el período académico.", ex);
            }
        }
    }
}
