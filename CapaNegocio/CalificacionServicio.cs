using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CalificacionServicio
    {
        CalificacionRepositorio repositorio = new CalificacionRepositorio();

        // RN-04: Las notas deben estar dentro del rango permitido (0 - NotaMaxima)
        // RN-06: NotaFinal = (Nota1 + Nota2) / 2
        // RN-07: Estado se calcula según el porcentaje de NotaMaxima
        //        >= 70% Aprobado | 50% - 69% Supletorio | < 50% Reprobado
        public void Guardar(Calificacion c)
        {
            Validar(c);

            c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
            c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

            repositorio.Insertar(c);
        }

        public void Actualizar(Calificacion c)
        {
            Validar(c);

            c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
            c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

            repositorio.Actualizar(c);
        }

        public void Eliminar(int idCalificacion)
        {
            if (idCalificacion <= 0)
                throw new ArgumentException("El identificador de la calificación no es válido.");

            repositorio.Eliminar(idCalificacion);
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