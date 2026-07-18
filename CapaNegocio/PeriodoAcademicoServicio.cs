using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de períodos académicos.
    /// Aplica validaciones de integridad, controla permisos y registra auditoría.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class PeriodoAcademicoServicio
    {
        private readonly PeriodoAcademicoRepositorio repositorio = new PeriodoAcademicoRepositorio();
        private readonly PermisoServicio permisoService = new PermisoServicio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Método auxiliar para validar período académico.
        /// </summary>
        private void ValidarPeriodo(PeriodoAcademico p)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p), "El período académico no puede ser nulo.");

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
        /// Registra un error en la bitácora. No modifica la estructura de la bitácora existente.
        /// </summary>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacoraService.RegistrarAccion(idUsuario, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde la bitácora para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra un nuevo período académico verificando permisos, validaciones y registrando auditoría.
        /// </summary>
        public void Guardar(PeriodoAcademico p, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar períodos académicos.");

                ValidarPeriodo(p);

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
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Periodos Academicos",
                    "Crear",
                    $"Intentando guardar período: Nombre='{(p != null ? p.NombrePeriodo : "null")}', FechaInicio={(p != null ? p.FechaInicio.ToString("o") : "null")}, FechaFin={(p != null ? p.FechaFin.ToString("o") : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar el período académico.", ex);
            }
        }

        /// <summary>
        /// Actualiza un período académico existente aplicando validaciones, control de permisos y auditoría.
        /// </summary>
        public void Actualizar(PeriodoAcademico p, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmPeriodosAcademicos", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar períodos académicos.");

                ValidarPeriodo(p);

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
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Periodos Academicos",
                    "Modificar",
                    $"Intentando actualizar período ID={(p != null ? p.IdPeriodo.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar el período académico.", ex);
            }
        }

        /// <summary>
        /// Elimina un período académico tras verificar permisos y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idPeriodo, int idUsuarioLogueado)
        {
            try
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
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Periodos Academicos", "Eliminar", $"Obteniendo nombre para ID={idPeriodo}");
                }

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
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Periodos Academicos",
                    "Eliminar",
                    $"Intentando eliminar período ID={idPeriodo}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
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
                RegistrarErrorEnBitacora(ex, 0, "Periodos Academicos", "ObtenerTodos", "Obteniendo todos los períodos académicos");
                throw new InvalidOperationException("Error al obtener los períodos académicos.", ex);
            }
        }

        /// <summary>
        /// Obtiene un período académico por su identificador.
        /// </summary>
        public PeriodoAcademico ObtenerPorId(int idPeriodo)
        {
            try
            {
                if (idPeriodo <= 0)
                    throw new ArgumentException("El identificador del período académico no es válido.");

                var periodo = repositorio.ObtenerPorId(idPeriodo);
                if (periodo == null)
                {
                    var ex = new InvalidOperationException($"No existe un período académico con ID {idPeriodo}.");
                    RegistrarErrorEnBitacora(ex, 0, "Periodos Academicos", "ObtenerPorId", $"ID no encontrado: {idPeriodo}");
                    throw ex;
                }

                return periodo;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Periodos Academicos", "ObtenerPorId", $"Obteniendo período ID={idPeriodo}");
                throw new InvalidOperationException("Error al obtener el período académico.", ex);
            }
        }
    }
}
