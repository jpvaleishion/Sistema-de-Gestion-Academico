namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa un rol de usuario dentro del sistema, utilizado para la gestión de permisos y accesos.
    /// </summary>
    public class Rol
    {
        // Propiedades que mapean directamente con las columnas de tu tabla 'Roles'

        /// <summary>
        /// Identificador único del rol.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre descriptivo del rol.
        /// </summary>
        public string NombreRol { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia vacía de la clase <see cref="Rol"/>. Esencial para serialización y mapeos.
        /// </summary>
        public Rol() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Rol"/> con los datos especificados.
        /// </summary>
        /// <param name="idRol">Identificador único del rol.</param>
        /// <param name="nombreRol">Nombre descriptivo del rol.</param>
        public Rol(int idRol, string nombreRol)
        {
            IdRol = idRol;
            NombreRol = nombreRol;
        }
    }
}