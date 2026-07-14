using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class BitacoraServicio
    {
        BitacoraRepositorio repositorio = new BitacoraRepositorio();

        public void RegistrarAccion(int idUsuario, string modulo, string accion, string descripcion)
        {
            Bitacora b = new Bitacora
            {
                IdUsuario = idUsuario,
                Modulo = modulo,
                Accion = accion,
                Descripcion = descripcion
            };

            repositorio.Insertar(b);
        }

        public List<Bitacora> ObtenerTodo()
        {
            return repositorio.ObtenerTodo();
        }
    }
}