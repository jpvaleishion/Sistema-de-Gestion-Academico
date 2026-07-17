using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de calificaciones académicas.
    /// Aplica reglas de negocio sobre notas, cálculo de nota final, estado y auditoría.
    /// </summary>
    public class CalificacionServicio
    {
        private CalificacionRepositorio repositorio = new CalificacionRepositorio();
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Registra una nueva calificación aplicando validaciones, cálculo de nota final y auditoría.
        /// </summary>
        public void Guardar(Calificacion c, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar calificaciones.");

            Validar(c);

            c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
            c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);
            c.FechaCalificacion = DateTime.Now;

            try
            {
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
                throw new InvalidOperationException("Error al guardar la calificación.", ex);
            }
        }

        /// <summary>
        /// Actualiza una calificación existente aplicando validaciones, recálculo y auditoría.
        /// </summary>
        public void Actualizar(Calificacion c, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar calificaciones.");

            Validar(c);

            c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
            c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

            try
            {
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
                throw new InvalidOperationException("Error al actualizar la calificación.", ex);
            }
        }

        /// <summary>
        /// Elimina una calificación y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idCalificacion, int idUsuarioLogueado)
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
            catch { }

            try
            {
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
                throw new InvalidOperationException("Error al obtener las calificaciones.", ex);
            }
        }

        /// <summary>
        /// Obtiene una calificación por su identificador.
        /// </summary>
        public Calificacion ObtenerPorId(int idCalificacion)
        {
            if (idCalificacion <= 0)
                throw new ArgumentException("El identificador de la calificación no es válido.");

            try
            {
                return repositorio.ObtenerPorId(idCalificacion);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener la calificación.", ex);
            }
        }

        // VALIDACIONES (RN-04)
        private void Validar(Calificacion c)
        {
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
