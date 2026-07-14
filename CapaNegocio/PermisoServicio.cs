using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class PermisoServicio
    {
        private PermisoRepositorio repositorio = new PermisoRepositorio();

        public List<Permiso> ObtenerPermisosPorRol(int idRol)
        {
            if (idRol <= 0)
                throw new ArgumentException("El identificador del rol no es válido.");

            return repositorio.ObtenerPermisosPorRol(idRol);
        }

        public bool TienePermiso(int idUsuario, string nombreFormulario, string operacion)
        {
            // Validaciones rápidas de seguridad
            if (idUsuario <= 0) return false;
            if (string.IsNullOrWhiteSpace(nombreFormulario)) return false;
            if (string.IsNullOrWhiteSpace(operacion)) return false;

            return repositorio.TienePermiso(idUsuario, nombreFormulario, operacion);
        }
    }
}