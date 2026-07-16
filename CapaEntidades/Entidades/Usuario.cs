using System;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa a un usuario del sistema, incluyendo sus credenciales de acceso, datos de seguridad y control de estado.
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

        /// <summary>
        /// Identificador del rol asignado al usuario.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del rol del usuario, utilizado para mostrar "Administrador" o "Docente" en el diseño.
        /// </summary>
        public string NombreRol { get; set; }

        // -- Nueva estructura de control de estados --

        /// <summary>
        /// Identificador del estado actual del usuario (FK referenciando a EstadosUsuario).
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// Nombre descriptivo del estado (ej. "Activo", "Inactivo", "Baneado") traído mediante JOIN.
        /// </summary>
        public string NombreEstado { get; set; }

        /// <summary>
        /// Razón o motivo específico por el cual el usuario tiene el estado actual.
        /// </summary>
        public string MotivoEstado { get; set; }

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
        /// <param name="idEstado">Identificador del estado actual del usuario.</param>
        /// <param name="nombreEstado">Nombre del estado del usuario.</param>
        /// <param name="motivoEstado">Razón del estado asignado.</param>
        /// <param name="salt">Valor aleatorio utilizado en el cifrado de la contraseña.</param>
        /// <param name="intentosFallidos">Número de intentos fallidos de inicio de sesión registrados.</param>
        /// <param name="fechaBloqueo">Fecha en la que el usuario fue bloqueado, si aplica.</param>
        public Usuario(int idUsuario, string nombreUsuario, string password, int idRol, string nombreRol, int idEstado, string nombreEstado, string motivoEstado, string salt, int intentosFallidos, DateTime? fechaBloqueo)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Password = password;
            IdRol = idRol;
            NombreRol = nombreRol;
            IdEstado = idEstado;
            NombreEstado = nombreEstado;
            MotivoEstado = motivoEstado;
            Salt = salt;
            IntentosFallidos = intentosFallidos;
            FechaBloqueo = fechaBloqueo;
        }
    }
}