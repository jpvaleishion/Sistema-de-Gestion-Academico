using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de calificaciones académicas.
    /// Aplica reglas de negocio sobre notas, cálculo de nota final, estado y auditoría.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class CalificacionServicio
    {
        private readonly CalificacionRepositorio repositorio = new CalificacionRepositorio();
        private readonly PermisoServicio permisoService = new PermisoServicio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();

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
                // No propagar excepciones desde el registro de errores para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra una nueva calificación aplicando validaciones, cálculo de nota final y auditoría.
        /// </summary>
        public void Guardar(Calificacion c, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar calificaciones.");

                Validar(c);

                c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
                c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);
                c.FechaCalificacion = DateTime.Now;

                repositorio.Insertar(c);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Crear",
                    $"Se registró calificación para Matrícula ID {c.IdMatricula}. Nota1: {c.Nota1}, Nota2: {c.Nota2}, NotaFinal: {c.NotaFinal} ({c.Estado})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Crear",
                    $"Intentando guardar calificación MatrículaID={(c != null ? c.IdMatricula.ToString() : "null")}, Nota1={(c != null ? c.Nota1.ToString() : "null")}, Nota2={(c != null ? c.Nota2.ToString() : "null")}"
                );

                // Mantener comportamiento previo: envolver en InvalidOperationException si no lo es ya
                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar la calificación.", ex);
            }
        }

        /// <summary>
        /// Actualiza una calificación existente aplicando validaciones, recálculo y auditoría.
        /// </summary>
        public void Actualizar(Calificacion c, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar calificaciones.");

                Validar(c);

                c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
                c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

                repositorio.Actualizar(c);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Modificar",
                    $"Se actualizó la Calificación ID {c.IdCalificacion} (Matrícula ID: {c.IdMatricula}). Nueva NotaFinal: {c.NotaFinal} ({c.Estado})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Modificar",
                    $"Intentando actualizar calificación ID={(c != null ? c.IdCalificacion.ToString() : "null")}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar la calificación.", ex);
            }
        }

        /// <summary>
        /// Elimina una calificación y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idCalificacion, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Eliminar"))
                    throw new InvalidOperationException("No tiene permisos para eliminar calificaciones.");

                if (idCalificacion <= 0)
                    throw new ArgumentException("El identificador de la calificación no es válido.");

                string detalleCalificacion = $"ID {idCalificacion}";
                try
                {
                    var califPrev = repositorio.ObtenerPorId(idCalificacion);
                    if (califPrev != null)
                        detalleCalificacion = $"ID {idCalificacion} (Matrícula ID: {califPrev.IdMatricula}, NotaFinal previa: {califPrev.NotaFinal})";
                }
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Calificaciones", "Eliminar", $"Obteniendo detalle para ID={idCalificacion}");
                }

                repositorio.Eliminar(idCalificacion);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Eliminar",
                    $"Se eliminó la calificación: {detalleCalificacion}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Eliminar",
                    $"Intentando eliminar calificación ID={idCalificacion}"
                );

                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al eliminar la calificación.", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las calificaciones registradas.
        /// </summary>
        public List<Calificacion> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Calificaciones", "ObtenerTodos", "Obteniendo todas las calificaciones");
                throw new InvalidOperationException("Error al obtener las calificaciones.", ex);
            }
        }

        /// <summary>
        /// Obtiene una calificación por su identificador.
        /// </summary>
        public Calificacion ObtenerPorId(int idCalificacion)
        {
            try
            {
                if (idCalificacion <= 0)
                    throw new ArgumentException("El identificador de la calificación no es válido.");

                return repositorio.ObtenerPorId(idCalificacion);
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Calificaciones", "ObtenerPorId", $"Obteniendo calificación ID={idCalificacion}");
                throw new InvalidOperationException("Error al obtener la calificación.", ex);
            }
        }

        // VALIDACIONES (RN-04)
        private void Validar(Calificacion c)
        {
            if (c == null)
                throw new ArgumentNullException(nameof(c), "La calificación no puede ser nula.");

            if (c.IdMatricula <= 0)
                throw new ArgumentException("Debe seleccionar una matrícula.");

            if (c.NotaMaxima <= 0)
                throw new ArgumentException("La nota máxima debe ser mayor a 0.");

            if (c.Nota1 < 0 || c.Nota1 > c.NotaMaxima)
                throw new ArgumentException("Nota1 debe estar entre 0 y la nota máxima.");

            if (c.Nota2 < 0 || c.Nota2 > c.NotaMaxima)
                throw new ArgumentException("Nota2 debe estar entre 0 y la nota máxima.");

            if (!string.IsNullOrWhiteSpace(c.Observaciones) && c.Observaciones.Length > 250)
                throw new ArgumentException("Las observaciones no pueden superar los 250 caracteres.");
        }

        // CALCULO NOTA FINAL (RN-06)
        private decimal CalcularNotaFinal(decimal nota1, decimal nota2)
        {
            return Math.Round((nota1 + nota2) / 2m, 2);
        }

        // CALCULO ESTADO ACADEMICO (RN-07)
        private string CalcularEstado(decimal notaFinal, decimal notaMaxima)
        {
            if (notaMaxima <= 0)
                throw new InvalidOperationException("La nota máxima debe ser mayor a 0 para calcular el estado.");

            decimal porcentaje = (notaFinal / notaMaxima) * 100m;

            if (porcentaje >= 70m)
                return "Aprobado";
            else if (porcentaje >= 50m)
                return "Supletorio";
            else
                return "Reprobado";
        }
    }
}
