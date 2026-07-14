using CapaDatos;
using CapaEntidades.Entidades;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class RolServicio
    {
        private RolRepositorio repositorio = new RolRepositorio();

        // Obtiene la lista de roles procesada desde el repositorio
        public List<Rol> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }
    }
}