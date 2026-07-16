using System;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa a un usuario del sistema, incluyendo sus credenciales de acceso y datos de seguridad.
    /// </summary>
    public class Usuario
    {
        // -- Propiedades base del sistema --

        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Nombre de usuario utilizado para el inicio de sesión.
        /// </summary>
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Contraseña del usuario, almacenada de forma segura.
        /// </summary>
        public string Password { get; set; }

        // *cambio* - Ahora usamos la relación de la base de datos

        /// <summary>
        /// Identificador del rol asignado al usuario.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del rol del usuario, utilizado para mostrar "Administrador" o "Docente" en el diseño.
        /// </summary>
        public string NombreRol { get; set; } // Para mostrar "Administrador" o "Docente" en el diseño

        /// <summary>
        /// Estado actual del usuario (por ejemplo, activo o bloqueado).
        /// </summary>
        public string Estado { get; set; }

        // -- CAMPOS PARA SEGURIDAD AVANZADA --

        /// <summary>
        /// Valor aleatorio (salt) utilizado en el proceso de cifrado de la contraseña.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Número de intentos fallidos de inicio de sesión registrados.
        /// </summary>
        public int IntentosFallidos { get; set; }

        /// <summary>
        /// Fecha en la que el usuario fue bloqueado por intentos fallidos, si aplica.
        /// </summary>
        public DateTime? FechaBloqueo { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Usuario"/>.
        /// </summary>
        public Usuario() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Usuario"/> con los datos especificados.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario.</param>
        /// <param name="nombreUsuario">Nombre de usuario utilizado para el inicio de sesión.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <param name="idRol">Identificador del rol asignado al usuario.</param>
        /// <param name="nombreRol">Nombre del rol del usuario.</param>
        /// <param name="estado">Estado actual del usuario.</param>
        /// <param name="salt">Valor aleatorio utilizado en el cifrado de la contraseña.</param>
        /// <param name="intentosFallidos">Número de intentos fallidos de inicio de sesión registrados.</param>
        /// <param name="fechaBloqueo">Fecha en la que el usuario fue bloqueado, si aplica.</param>
        public Usuario(int idUsuario, string nombreUsuario, string password, int idRol, string nombreRol, string estado, string salt, int intentosFallidos, DateTime? fechaBloqueo)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Password = password;
            IdRol = idRol;
            NombreRol = nombreRol;
            Estado = estado;
            Salt = salt;
            IntentosFallidos = intentosFallidos;
            FechaBloqueo = fechaBloqueo;
        }
    }
}