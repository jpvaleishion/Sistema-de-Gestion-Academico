using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CapaNegocio
{
    public class UsuarioServicio
    {
        UsuarioRepositorio repositorio = new UsuarioRepositorio();
        BitacoraServicio bitacoraNegocio = new BitacoraServicio();

        private const int MAX_INTENTOS_FALLIDOS = 3;
        private const int MINUTOS_BLOQUEO = 15;
        private const string MENSAJE_LOGIN_GENERICO = "Usuario o contraseña incorrectos.";

        public void Guardar(Usuario u, int idUsuarioLogueado)
        {
            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            // *cambio* - Validamos longitud en lugar de formato de correo
            if (!EsNombreUsuarioValido(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario debe tener entre 3 y 100 caracteres.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (u.IdRol <= 0)
                throw new ArgumentException("El rol es obligatorio.");

            string salt = GenerarSalt();
            string passwordEncriptada = EncriptarPassword(u.Password, salt);

            u.Salt = salt;
            u.Password = passwordEncriptada;
            u.IntentosFallidos = 0;
            u.FechaBloqueo = null;

            repositorio.Insertar(u);

            bitacoraNegocio.RegistrarAccion(
                idUsuarioLogueado,
                "Usuarios",
                "Crear Usuario",
                $"Se creó exitosamente el usuario {u.NombreUsuario}."
            );
        }

        public void Actualizar(Usuario u, int idUsuarioLogueado)
        {
            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            // *cambio* - Validamos longitud en lugar de formato de correo
            if (!EsNombreUsuarioValido(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario debe tener entre 3 y 100 caracteres.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (u.IdRol <= 0)
                throw new ArgumentException("El rol es obligatorio.");

            string salt = GenerarSalt();
            string passwordEncriptada = EncriptarPassword(u.Password, salt);

            u.Salt = salt;
            u.Password = passwordEncriptada;

            repositorio.Actualizar(u);

            bitacoraNegocio.RegistrarAccion(
                idUsuarioLogueado,
                "Usuarios",
                "Actualizar Usuario",
                $"Se actualizaron los datos del usuario {u.NombreUsuario}."
            );
        }

        public void Eliminar(int idUsuario, int idUsuarioLogueado)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.");

            var u = repositorio.ObtenerPorId(idUsuario);
            string nombreUsuarioEliminado = u != null ? u.NombreUsuario : "Desconocido";

            repositorio.Eliminar(idUsuario);

            bitacoraNegocio.RegistrarAccion(
                idUsuarioLogueado,
                "Usuarios",
                "Eliminar Usuario",
                $"Se eliminó de forma permanente al usuario con ID {idUsuario} (Nombre: {nombreUsuarioEliminado})."
            );
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

        public Usuario IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es obligatoria.");

            Usuario u = repositorio.ObtenerPorNombreUsuario(nombreUsuario);

            if (u == null)
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);

            if (u.FechaBloqueo.HasValue && u.FechaBloqueo.Value > DateTime.Now)
            {
                bitacoraNegocio.RegistrarAccion(
                   u.IdUsuario,
                   "Security",
                   "Acceso Denegado",
                   "Intento de acceso en una cuenta que se encuentra bloqueada.");
                TimeSpan restante = u.FechaBloqueo.Value - DateTime.Now;
                throw new InvalidOperationException(
                    $"La cuenta está bloqueada temporalmente. Intente nuevamente en {Math.Ceiling(restante.TotalMinutes)} minuto(s).");
            }

            string passwordEncriptada = EncriptarPassword(password, u.Salt);

            if (u.Password != passwordEncriptada)
            {
                int intentos = u.IntentosFallidos + 1;
                DateTime? fechaBloqueo = null;

                if (intentos >= MAX_INTENTOS_FALLIDOS)
                {
                    fechaBloqueo = DateTime.Now.AddMinutes(MINUTOS_BLOQUEO);
                }

                repositorio.ActualizarIntentosYBloqueo(u.IdUsuario, intentos, fechaBloqueo);
                bitacoraNegocio.RegistrarAccion(
                    u.IdUsuario,
                    "Seguridad",
                    "Intento Fallido",
                    $"Contraseña incorrecta. Intento #{intentos} para el usuario {u.NombreUsuario}.");
                if (fechaBloqueo.HasValue)
                {
                    bitacoraNegocio.RegistrarAccion(
                        u.IdUsuario,
                        "Seguridad",
                        "Bloqueo de Cuenta",
                        "Cuenta bloqueada temporalmente por exceso de intentos fallidos.");
                }
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);
            }

            if (u.Estado != "Activo")
                throw new InvalidOperationException("El usuario está inactivo.");

            repositorio.ActualizarIntentosYBloqueo(u.IdUsuario, 0, null);
            u.IntentosFallidos = 0;
            u.FechaBloqueo = null;

            bitacoraNegocio.RegistrarAccion(
                u.IdUsuario,
                "Seguridad",
                "Inicio de Sesión",
                $"El usuario {u.NombreUsuario} ingresó al sistema con éxito.");

            return u;
        }

        private string GenerarSalt()
        {
            byte[] bytesSalt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytesSalt);
            }
            return Convert.ToBase64String(bytesSalt);
        }

        private string EncriptarPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesEntrada = Encoding.UTF8.GetBytes(password + salt);
                byte[] bytesHash = sha256.ComputeHash(bytesEntrada);
                return Convert.ToBase64String(bytesHash);
            }
        }

        // *cambio* - Nueva validación de nombre de usuario flexible (reemplaza a EsEmailValido)
        private bool EsNombreUsuarioValido(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            // Permite cualquier tipo de nombre de usuario (correos, nicks, etc.) entre 3 y 100 caracteres.
            return nombreUsuario.Length >= 3 && nombreUsuario.Length <= 100;
        }
    }
}