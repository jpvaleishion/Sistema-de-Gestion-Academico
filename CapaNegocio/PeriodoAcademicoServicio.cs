using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class PeriodoAcademicoServicio
    {
        PeriodoAcademicoRepositorio repositorio = new PeriodoAcademicoRepositorio();

        public void Guardar(PeriodoAcademico p)
        {
            if (string.IsNullOrWhiteSpace(p.NombrePeriodo))
                throw new ArgumentException("El nombre del período es obligatorio.");

            if (p.FechaFin <= p.FechaInicio)
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");

            repositorio.Insertar(p);
        }

        public void Actualizar(PeriodoAcademico p)
        {
            if (string.IsNullOrWhiteSpace(p.NombrePeriodo))
                throw new ArgumentException("El nombre del período es obligatorio.");

            if (p.FechaFin <= p.FechaInicio)
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");

            repositorio.Actualizar(p);
        }

        public void Eliminar(int idPeriodo)
        {
            if (idPeriodo <= 0)
                throw new ArgumentException("El identificador del período académico no es válido.");

            repositorio.Eliminar(idPeriodo);
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