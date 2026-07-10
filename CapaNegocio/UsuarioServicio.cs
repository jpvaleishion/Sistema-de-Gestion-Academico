using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class UsuarioServicio
    {
        UsuarioRepositorio repositorio = new UsuarioRepositorio();

        public void Guardar(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (string.IsNullOrWhiteSpace(u.Rol))
                throw new ArgumentException("El rol es obligatorio.");

            repositorio.Insertar(u);
        }

        public void Actualizar(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (string.IsNullOrWhiteSpace(u.Rol))
                throw new ArgumentException("El rol es obligatorio.");

            repositorio.Actualizar(u);
        }

        public void Eliminar(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.");

            repositorio.Eliminar(idUsuario);
        }

        public List<Usuario> ObtenerTodos()
        {
            return repositorio.ObtenerTodos();
        }

        public Usuario ObtenerPorId(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.");

            return repositorio.ObtenerPorId(idUsuario);
        }

        // RF-08: Iniciar sesión
        public Usuario IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es obligatoria.");

            Usuario u = repositorio.ObtenerPorNombreUsuario(nombreUsuario);

            if (u == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            if (u.Password != password)
                throw new InvalidOperationException("Contraseña incorrecta.");

            if (u.Estado != "Activo")
                throw new InvalidOperationException("El usuario está inactivo.");

            return u;
        }
    }
}