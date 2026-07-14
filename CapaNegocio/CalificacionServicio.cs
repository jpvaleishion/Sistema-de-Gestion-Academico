using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CalificacionServicio
    {
        private CalificacionRepositorio repositorio = new CalificacionRepositorio();

        // *cambio* - Instanciamos los servicios de permisos y bitácora
        private PermisoServicio permisoService = new PermisoServicio();
        private BitacoraServicio bitacoraService = new BitacoraServicio();

        // RN-04: Las notas deben estar dentro del rango permitido (0 - NotaMaxima)
        // RN-06: NotaFinal = (Nota1 + Nota2) / 2
        // RN-07: Estado se calcula según el porcentaje de NotaMaxima
        //        >= 70% Aprobado | 50% - 69% Supletorio | < 50% Reprobado

        // *cambio* - Ahora recibe el ID del usuario logueado para validar seguridad y registrar auditoría
        public void Guardar(Calificacion c, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Crear calificaciones
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Crear"))
            {
                throw new InvalidOperationException("No tiene permisos para registrar calificaciones.");
            }

            Validar(c);

            c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
            c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

            repositorio.Insertar(c);

            // *auditoria* - Se registra la acción en la bitácora con los detalles de la nota
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Calificaciones",
                "Crear",
                $"Se registró calificación para la Matrícula ID {c.IdMatricula}. Nota 1: {c.Nota1}, Nota 2: {c.Nota2}, Nota Final: {c.NotaFinal} ({c.Estado})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Actualizar(Calificacion c, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Modificar calificaciones
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Modificar"))
            {
                throw new InvalidOperationException("No tiene permisos para modificar calificaciones.");
            }

            Validar(c);

            c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
            c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

            repositorio.Actualizar(c);

            // *auditoria* - Se registra la actualización en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Calificaciones",
                "Modificar",
                $"Se actualizó la Calificación ID {c.IdCalificacion} (Matrícula ID: {c.IdMatricula}). Nueva Nota Final: {c.NotaFinal} ({c.Estado})."
            );
        }

        // *cambio* - Ahora recibe el ID del usuario logueado
        public void Eliminar(int idCalificacion, int idUsuarioLogueado)
        {
            // *seguridad* - Validar si el rol tiene permiso para Eliminar calificaciones
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Eliminar"))
            {
                throw new InvalidOperationException("No tiene permisos para eliminar calificaciones.");
            }

            if (idCalificacion <= 0)
                throw new ArgumentException("El identificador de la calificación no es válido.");

            // Opcional: Obtener información de la calificación antes de borrar para la descripción de la bitácora
            string detalleCalificacion = $"ID {idCalificacion}";
            try
            {
                var califPrev = repositorio.ObtenerPorId(idCalificacion);
                if (califPrev != null)
                {
                    detalleCalificacion = $"ID {idCalificacion} (Matrícula ID: {califPrev.IdMatricula}, Nota Final previa: {califPrev.NotaFinal})";
                }
            }
            catch { /* Continuar si falla la lectura preliminar */ }

            repositorio.Eliminar(idCalificacion);

            // *auditoria* - Se registra la eliminación en la bitácora
            bitacoraService.RegistrarAccion(
                idUsuarioLogueado,
                "Calificaciones",
                "Eliminar",
                $"Se eliminó la calificación: {detalleCalificacion}."
            );
        }

        public List<Calificacion> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Calificacion ObtenerPorId(int idCalificacion)
        {
            if (idCalificacion <= 0)
                throw new ArgumentException("El identificador de la calificación no es válido.");

            return repositorio.ObtenerPorId(idCalificacion);
        }

        // ── VALIDACIONES (RN-04) ──────────────────────────────────────
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
        }

        // ── CALCULO NOTA FINAL (RN-06) ────────────────────────────────
        private decimal CalcularNotaFinal(decimal nota1, decimal nota2)
        {
            return (nota1 + nota2) / 2m;
        }

        // ── CALCULO ESTADO ACADEMICO (RN-07) ──────────────────────────
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