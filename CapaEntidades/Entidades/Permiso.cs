namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa el permiso asignado a un rol sobre un formulario o módulo específico del sistema.
    /// </summary>
    public class Permiso
    {
        /// <summary>
        /// Identificador único del permiso.
        /// </summary>
        public int IdPermiso { get; set; }

        /// <summary>
        /// Identificador del rol al que pertenece el permiso.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Identificador del menú asociado al permiso.
        /// </summary>
        public int IdMenu { get; set; }

        /// <summary>
        /// Nombre del formulario al que aplica el permiso. Ej: "frmEstudiantes".
        /// </summary>
        public string NombreFormulario { get; set; } // Ej: "frmEstudiantes"

        /// <summary>
        /// Indica si el rol tiene permiso para visualizar el formulario.
        /// </summary>
        public bool Visualizar { get; set; }

        /// <summary>
        /// Indica si el rol tiene permiso para crear registros en el formulario.
        /// </summary>
        public bool Crear { get; set; }

        /// <summary>
        /// Indica si el rol tiene permiso para modificar registros en el formulario.
        /// </summary>
        public bool Modificar { get; set; }

        /// <summary>
        /// Indica si el rol tiene permiso para eliminar registros en el formulario.
        /// </summary>
        public bool Eliminar { get; set; }
    }
}