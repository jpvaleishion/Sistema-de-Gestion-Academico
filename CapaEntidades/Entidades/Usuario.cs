using System;

namespace CapaEntidades.Entidades
{
    public class Usuario
    {
        public Usuario() { }

        // -- Propiedades base del sistema --
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }

        // *cambio* - Ahora usamos la relación de la base de datos
        public int IdRol { get; set; }
        public string NombreRol { get; set; } // Para mostrar "Administrador" o "Docente" en el diseño

        public string Estado { get; set; }

        // -- CAMPOS PARA SEGURIDAD AVANZADA --
        public string Salt { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? FechaBloqueo { get; set; }

        // *cambio* - Constructor adaptado
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