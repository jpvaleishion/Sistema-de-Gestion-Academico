namespace CapaEntidades.Entidades
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public int IdRol { get; set; }
        public int IdMenu { get; set; }
        public string NombreFormulario { get; set; } // Ej: "frmEstudiantes"
        public bool Visualizar { get; set; }
        public bool Crear { get; set; }
        public bool Modificar { get; set; }
        public bool Eliminar { get; set; }
    }
}