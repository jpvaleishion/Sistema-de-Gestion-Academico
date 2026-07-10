using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.Entidades
{
    public class Usuario
    {
        public Usuario() { }

        // -- Propiedades base del sistema --
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; } //Aqui se guarda el Hash
        public string Rol { get; set; }
        public string Estado { get; set; }

        // -- CAMPOS PARA SEGURIDAD AVANZADA --
        public string Salt { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? FechaBloqueo { get; set; } // '?' para valores nullables, ya que un usuario puede no estar bloqueado

        // -- Constructor completo --
        public Usuario(int idUsuario, string nombreUsuario, string password, string rol, string estado, string salt, int intentosFallidos, DateTime? fechaBloqueo)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Password = password;
            Rol = rol;
            Estado = estado;
            Salt = salt;
            IntentosFallidos = intentosFallidos;
            FechaBloqueo = fechaBloqueo;
        }
    }
}
