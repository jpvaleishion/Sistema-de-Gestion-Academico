using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class UsuarioServicio
    {
        UsuarioRepositorio repositorio = new UsuarioRepositorio();

        private const int MAX_INTENTOS_FALLIDOS = 3;
        private const int MINUTOS_BLOQUEO = 15;
        private const string MENSAJE_LOGIN_GENERICO = "Usuario o contraseña incorrectos.";

        public void Guardar(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (!EsEmailValido(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario debe tener un formato de correo electrónico válido.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (string.IsNullOrWhiteSpace(u.Rol))
                throw new ArgumentException("El rol es obligatorio.");

            string salt = GenerarSalt();
            string passwordEncriptada = EncriptarPassword(u.Password, salt);

            u.Salt = salt;
            u.Password = passwordEncriptada;
            u.IntentosFallidos = 0;
            u.FechaBloqueo = null;

            repositorio.Insertar(u);
        }

        public void Actualizar(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (!EsEmailValido(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario debe tener un formato de correo electrónico válido.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (string.IsNullOrWhiteSpace(u.Rol))
                throw new ArgumentException("El rol es obligatorio.");

            string salt = GenerarSalt();
            string passwordEncriptada = EncriptarPassword(u.Password, salt);

            u.Salt = salt;
            u.Password = passwordEncriptada;

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
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);

            // Verifica si la cuenta sigue bloqueada por tiempo
            if (u.FechaBloqueo.HasValue && u.FechaBloqueo.Value > DateTime.Now)
            {
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

                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);
            }

            if (u.Estado != "Activo")
                throw new InvalidOperationException("El usuario está inactivo.");

            // Ingreso exitoso: reinicia contadores de seguridad
            repositorio.ActualizarIntentosYBloqueo(u.IdUsuario, 0, null);
            u.IntentosFallidos = 0;
            u.FechaBloqueo = null;

            return u;
        }

        // ── SEGURIDAD: SALT Y HASH SHA256 ─────────────────────────────
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

        // ── VALIDACIÓN DE FORMATO DE CORREO ───────────────────────────
        private bool EsEmailValido(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            return Regex.IsMatch(nombreUsuario, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}