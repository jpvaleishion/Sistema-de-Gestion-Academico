namespace CapaEntidades.Entidades
{
    public class Rol
    {
        // Propiedades que mapean directamente con las columnas de tu tabla 'Roles'
        public int IdRol { get; set; }
        public string NombreRol { get; set; }

        // Constructor vacío (esencial para serialización y mapeos)
        public Rol() { }

        // Constructor con parámetros para instanciación rápida
        public Rol(int idRol, string nombreRol)
        {
            IdRol = idRol;
            NombreRol = nombreRol;
        }
    }
}