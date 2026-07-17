using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio responsable de la gestión de usuarios.
    /// Encapsula validaciones, encriptación avanzada (PBKDF2), control de intentos y auditoría.
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
        /// Crea un nuevo usuario: valida datos, genera salt, encripta contraseña con PBKDF2 y registra auditoría.
        /// </summary>
        public void Guardar(Usuario u, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Crear"))
                throw new InvalidOperationException("No tiene permisos para registrar usuarios.");

            if (u == null)
                throw new ArgumentNullException(nameof(u));

            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (!EsNombreUsuarioValido(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario debe tener entre 3 y 100 caracteres.");

            if (string.IsNullOrWhiteSpace(u.Password))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (u.IdRol <= 0)
                throw new ArgumentException("El rol es obligatorio.");

            if (u.IdEstado <= 0)
                throw new ArgumentException("El estado es obligatorio.");

            try
            {
                // Generamos un Salt único y aplicamos el hash robusto
                u.Salt = GenerarSalt();
                u.Password = EncriptarPassword(u.Password, u.Salt);

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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al guardar el usuario.", ex);
            }
        }

        /// <summary>
        /// Actualiza un usuario existente de forma segura. Si la contraseña no se modificó en el formulario,
        /// la mantiene intacta para evitar re-encriptar un hash existente.
        /// </summary>
        public void Actualizar(Usuario u, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Modificar"))
                throw new InvalidOperationException("No tiene permisos para modificar usuarios.");

            if (u == null)
                throw new ArgumentNullException(nameof(u));

            if (string.IsNullOrWhiteSpace(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (!EsNombreUsuarioValido(u.NombreUsuario))
                throw new ArgumentException("El nombre de usuario debe tener entre 3 y 100 caracteres.");

            if (u.IdRol <= 0)
                throw new ArgumentException("El rol es obligatorio.");

            if (u.IdEstado <= 0)
                throw new ArgumentException("El estado es obligatorio.");

            try
            {
                // Jalamos el estado actual del usuario desde la base de datos para comparar
                Usuario usuarioExistente = repositorio.ObtenerPorId(u.IdUsuario);
                if (usuarioExistente == null)
                    throw new InvalidOperationException("El usuario que intenta modificar ya no existe.");

                // PROTECCIÓN: Si desde la UI la contraseña viene vacía o viene el hash viejo idéntico,
                // significa que el usuario NO cambió su clave (solo editó el rol, nombre, estado, etc.)
                if (string.IsNullOrWhiteSpace(u.Password) || u.Password == usuarioExistente.Password)
                {
                    u.Salt = usuarioExistente.Salt;
                    u.Password = usuarioExistente.Password;
                }
                else
                {
                    // Si escribieron algo nuevo, asumimos que es texto plano y generamos nueva seguridad
                    u.Salt = GenerarSalt();
                    u.Password = EncriptarPassword(u.Password, u.Salt);
                }

                repositorio.Actualizar(u);

                bitacoraNegocio.RegistrarAccion(
                    idUsuarioLogueado,
                    "Usuarios",
                    "Actualizar Usuario",
                    $"Se actualizaron los datos del usuario {u.NombreUsuario}."
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al actualizar el usuario.", ex);
            }
        }

        /// <summary>
        /// Elimina permanentemente un usuario y registra la acción en la bitácora.
        /// </summary>
        public void Eliminar(int idUsuario, int idUsuarioLogueado)
        {
            if (!permisoService.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Eliminar"))
                throw new InvalidOperationException("No tiene permisos para eliminar usuarios.");

            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.");

            try
            {
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
            catch (Exception ex)
            {
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
                throw new InvalidOperationException("Error al obtener los usuarios.", ex);
            }
        }

        public Usuario ObtenerPorId(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.");

            try
            {
                var usuario = repositorio.ObtenerPorId(idUsuario);
                if (usuario == null)
                    throw new InvalidOperationException($"No existe un usuario con ID {idUsuario}.");
                return usuario;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener el usuario.", ex);
            }
        }

        /// <summary>
        /// Intenta iniciar sesión controlando intentos fallidos, bloqueos invisibles y verificación segura con PBKDF2.
        /// </summary>
        public Usuario IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es obligatoria.");

            Usuario u;
            try
            {
                u = repositorio.ObtenerPorNombreUsuario(nombreUsuario);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al verificar credenciales.", ex);
            }

            // 1. Si el usuario no existe, salimos con mensaje genérico (Evita enumeración de usuarios)
            if (u == null)
                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);

            // 2. Si está bloqueado por tiempo, registramos en bitácora pero mostramos error genérico al cliente
            if (u.FechaBloqueo.HasValue && u.FechaBloqueo.Value > DateTime.Now)
            {
                bitacoraNegocio.RegistrarAccion(
                    u.IdUsuario,
                    "Seguridad",
                    "Acceso Denegado",
                    $"Intento de acceso en cuenta bloqueada temporalmente para el usuario: {u.NombreUsuario}.");

                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);
            }

            // 3. Verificación criptográfica con PBKDF2 y tiempo constante manual
            if (!VerificarPassword(password, u.Salt, u.Password))
            {
                try
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
                            $"Cuenta del usuario {u.NombreUsuario} bloqueada temporalmente por exceder intentos.");
                    }
                }
                catch (Exception ex)
                {
                    // No revelar detalles al cliente; registrar y continuar con mensaje genérico
                    throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO, ex);
                }

                throw new InvalidOperationException(MENSAJE_LOGIN_GENERICO);
            }

            // 4. Validación de estado administrativo numérico (IdEstado)
            if (u.IdEstado != 1)
                throw new InvalidOperationException("El usuario está inactivo o suspendido.");

            // 5. Login Exitoso: Limpiamos bloqueos e intentos
            try
            {
                repositorio.ActualizarIntentosYBloqueo(u.IdUsuario, 0, null);
                u.IntentosFallidos = 0;
                u.FechaBloqueo = null;

                bitacoraNegocio.RegistrarAccion(
                    u.IdUsuario,
                    "Seguridad",
                    "Inicio de Sesión",
                    $"El usuario {u.NombreUsuario} ingresó al sistema con éxito.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al finalizar el proceso de inicio de sesión.", ex);
            }

            return u;
        }

        #region Métodos Privados Criptográficos

        /// <summary>
        /// Genera un salt criptográfico aleatorio de 16 bytes codificado en Base64.
        /// </summary>
        private string GenerarSalt()
        {
            byte[] bytesSalt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytesSalt);
            }
            return Convert.ToBase64String(bytesSalt);
        }

        /// <summary>
        /// Deriva la clave usando el algoritmo PBKDF2 con 100,000 iteraciones y SHA256.
        /// </summary>
        private string EncriptarPassword(string password, string saltBase64)
        {
            byte[] salt = Convert.FromBase64String(saltBase64);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); // Genera un hash de 256 bits (32 bytes)
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Compara de forma segura y en tiempo constante el intento de contraseña contra el hash de la BD.
        /// </summary>
        private bool VerificarPassword(string password, string saltBase64, string hashAlmacenadoBase64)
        {
            try
            {
                byte[] salt = Convert.FromBase64String(saltBase64);
                byte[] hashAlmacenado = Convert.FromBase64String(hashAlmacenadoBase64);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hashCalculado = pbkdf2.GetBytes(32);

                    // Comparación en tiempo constante
                    return CompararEnTiempoConstante(hashCalculado, hashAlmacenado);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reemplazo manual de FixedTimeEquals para evitar depender de .NET Core.
        /// Compara dos arreglos de bytes sin importar dónde difieran para evitar ataques de temporización.
        /// </summary>
        private bool CompararEnTiempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length)
                return false;

            int resultado = 0;
            for (int i = 0; i < a.Length; i++)
            {
                resultado |= a[i] ^ b[i]; // Operación XOR bit a bit
            }
            return resultado == 0;
        }

        private bool EsNombreUsuarioValido(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            return nombreUsuario.Length >= 3 && nombreUsuario.Length <= 100;
        }

        #endregion
    }
}
