using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio responsable de la gestión de usuarios.
    /// Encapsula validaciones, encriptación, control de intentos y auditoría.
    /// </summary>
    public class UsuarioServicio
    {
        private UsuarioRepositorio repositorio = new UsuarioRepositorio();
        private BitacoraServicio bitacoraNegocio = new BitacoraServicio();
        private PermisoServicio permisoService = new PermisoServicio();

        private const int MAX_INTENTOS_FALLIDOS = 3;
        private const int MINUTOS_BLOQUEO = 15;
        private const string MENSAJE_LOGIN_GENERICO = "Usuario o contraseña incorrectos.";

        /// <summary>
        /// Registra un error en la bitácora para centralizar el diagnóstico.
        /// </summary>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacoraNegocio.RegistrarAccion(idUsuario <= 0 ? 0 : idUsuario, modulo, "Error", descripcion);
            }
            catch { /* Evitar propagar errores de la bitácora */ }
        }

        public void Guardar(Usuario u, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar usuarios.");

            if (u == null) throw new ArgumentNullException(nameof(u));
            if (string.IsNullOrWhiteSpace(u.NombreUsuario)) throw new ArgumentException("El nombre de usuario es obligatorio.");
            if (!EsNombreUsuarioValido(u.NombreUsuario)) throw new ArgumentException("El nombre de usuario debe tener entre 3 y 100 caracteres.");
            if (string.IsNullOrWhiteSpace(u.Password)) throw new ArgumentException("La contraseña es obligatoria.");
            if (u.IdRol <= 0) throw new ArgumentException("El rol es obligatorio.");
            if (u.IdEstado <= 0) throw new ArgumentException("El estado es obligatorio.");

            try
            {
                u.Salt = GenerarSalt();
                u.Password = EncriptarPassword(u.Password, u.Salt);
                u.IntentosFallidos = 0;
                u.FechaBloqueo = null;

                repositorio.Insertar(u);

                bitacoraNegocio.RegistrarAccion(idUsuarioLogueado, "Usuarios", "Crear Usuario", $"Se creó exitosamente el usuario {u.NombreUsuario}.");
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, idUsuarioLogueado, "Usuarios", "Crear Usuario", $"Error al guardar usuario: {u.NombreUsuario}");
                throw new InvalidOperationException("Error al guardar el usuario.", ex);
            }
        }

        public void Actualizar(Usuario u, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar usuarios.");

            if (u == null) throw new ArgumentNullException(nameof(u));
            if (string.IsNullOrWhiteSpace(u.NombreUsuario)) throw new ArgumentException("El nombre de usuario es obligatorio.");
            if (!EsNombreUsuarioValido(u.NombreUsuario)) throw new ArgumentException("El nombre de usuario debe tener entre 3 y 100 caracteres.");
            if (u.IdRol <= 0) throw new ArgumentException("El rol es obligatorio.");
            if (u.IdEstado <= 0) throw new ArgumentException("El estado es obligatorio.");

            try
            {
                Usuario usuarioExistente = repositorio.ObtenerPorId(u.IdUsuario);
                if (usuarioExistente == null) throw new InvalidOperationException("El usuario que intenta modificar ya no existe.");

                if (string.IsNullOrWhiteSpace(u.Password) || u.Password == usuarioExistente.Password)
                {
                    u.Salt = usuarioExistente.Salt;
                    u.Password = usuarioExistente.Password;
                }
                else
                {
                    u.Salt = GenerarSalt();
                    u.Password = EncriptarPassword(u.Password, u.Salt);
                }

                repositorio.Actualizar(u);

                bitacoraNegocio.RegistrarAccion(idUsuarioLogueado, "Usuarios", "Actualizar Usuario", $"Se actualizaron los datos del usuario {u.NombreUsuario}.");
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, idUsuarioLogueado, "Usuarios", "Actualizar Usuario", $"Error actualizando ID {u.IdUsuario}");
                throw new InvalidOperationException("Error al actualizar el usuario.", ex);
            }
        }

        public void Eliminar(int idUsuario, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Eliminar"))
                throw new InvalidOperationException("No tiene permisos para eliminar usuarios.");

            if (idUsuario <= 0) throw new ArgumentException("El identificador del usuario no es válido.");

            try
            {
                var u = repositorio.ObtenerPorId(idUsuario);
                string nombre = u != null ? u.NombreUsuario : "Desconocido";

                repositorio.Eliminar(idUsuario);

                bitacoraNegocio.RegistrarAccion(idUsuarioLogueado, "Usuarios", "Eliminar Usuario", $"Se eliminó al usuario {nombre} (ID: {idUsuario}).");
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, idUsuarioLogueado, "Usuarios", "Eliminar Usuario", $"Error eliminando ID {idUsuario}");
                throw new InvalidOperationException("Error al eliminar el usuario.", ex);
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            try
            {
                var lista = repositorio.ObtenerTodos();
                if (lista == null || lista.Count == 0)
                    throw new InvalidOperationException("No se encontraron usuarios registrados.");
                return lista;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Usuarios", "ObtenerTodos", "Error al listar usuarios.");
                throw new InvalidOperationException("Error al obtener los usuarios.", ex);
            }
        }

        public Usuario ObtenerPorId(int idUsuario)
        {
            if (idUsuario <= 0) throw new ArgumentException("El identificador del usuario no es válido.");

            try
            {
                var usuario = repositorio.ObtenerPorId(idUsuario);
                if (usuario == null) throw new InvalidOperationException($"No existe un usuario con ID {idUsuario}.");
                return usuario;
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Usuarios", "ObtenerPorId", $"Error obteniendo ID {idUsuario}");
                throw new InvalidOperationException("Error al obtener el usuario.", ex);
            }
        }

        public Usuario IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Credenciales incompletas.");

            Usuario u;
            try
            {
                u = repositorio.ObtenerPorNombreUsuario(nombreUsuario);
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Seguridad", "Login", $"Fallo en consulta BD para usuario {nombreUsuario}");
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO, ex);
            }

            if (u == null) throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);

            if (u.FechaBloqueo.HasValue && u.FechaBloqueo.Value > DateTime.Now)
            {
                bitacoraNegocio.RegistrarAccion(u.IdUsuario, "Seguridad", "Acceso Denegado", "Intento en cuenta bloqueada.");
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);
            }

            if (!VerificarPassword(password, u.Salt, u.Password))
            {
                try
                {
                    int intentos = u.IntentosFallidos + 1;
                    DateTime? bloqueo = (intentos >= MAX_INTENTOS_FALLIDOS) ? (DateTime?)DateTime.Now.AddMinutes(MINUTOS_BLOQUEO) : null;
                    repositorio.ActualizarIntentosYBloqueo(u.IdUsuario, intentos, bloqueo);
                    bitacoraNegocio.RegistrarAccion(u.IdUsuario, "Seguridad", "Intento Fallido", $"Intento #{intentos} para {u.NombreUsuario}.");
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(ex, u.IdUsuario, "Seguridad", "Login", "Fallo al registrar intento fallido.");
                }
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);
            }

            if (u.IdEstado != 1) throw new InvalidOperationException("El usuario está inactivo.");

            try
            {
                repositorio.ActualizarIntentosYBloqueo(u.IdUsuario, 0, null);
                bitacoraNegocio.RegistrarAccion(u.IdUsuario, "Seguridad", "Login", "Inicio de sesión exitoso.");
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, u.IdUsuario, "Seguridad", "Login", "Error al resetear intentos post-login.");
            }

            return u;
        }

        #region Métodos Criptográficos
        private string GenerarSalt()
        {
            byte[] bytesSalt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) { rng.GetBytes(bytesSalt); }
            return Convert.ToBase64String(bytesSalt);
        }

        private string EncriptarPassword(string password, string saltBase64)
        {
            byte[] salt = Convert.FromBase64String(saltBase64);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(pbkdf2.GetBytes(32));
            }
        }

        private bool VerificarPassword(string password, string saltBase64, string hashAlmacenadoBase64)
        {
            try
            {
                byte[] salt = Convert.FromBase64String(saltBase64);
                byte[] hashAlmacenado = Convert.FromBase64String(hashAlmacenadoBase64);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    return CompararEnTiempoConstante(pbkdf2.GetBytes(32), hashAlmacenado);
                }
            }
            catch { return false; }
        }

        private bool CompararEnTiempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int resultado = 0;
            for (int i = 0; i < a.Length; i++) { resultado |= a[i] ^ b[i]; }
            return resultado == 0;
        }

        private bool EsNombreUsuarioValido(string nombreUsuario)
        {
            return !string.IsNullOrWhiteSpace(nombreUsuario) && nombreUsuario.Length >= 3 && nombreUsuario.Length <= 100;
        }
        #endregion
    }
}