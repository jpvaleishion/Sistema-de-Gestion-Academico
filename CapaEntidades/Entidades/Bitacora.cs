using System;

namespace CapaEntidades.Entidades
{
    /// <summary>
    /// Representa un registro de auditoría que documenta las acciones realizadas por los usuarios dentro del sistema.
    /// </summary>
    public class Bitacora
    {
        /// <summary>
        /// Identificador único del registro de bitácora.
        /// </summary>
        public int IdBitacora { get; set; }

        /// <summary>
        /// Identificador del usuario que ejecutó la acción registrada.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Fecha y hora exacta en que se realizó la acción.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Módulo del sistema donde se originó la acción registrada.
        /// </summary>
        public string Modulo { get; set; }

        /// <summary>
        /// Acción específica realizada por el usuario.
        /// </summary>
        public string Accion { get; set; }

        /// <summary>
        /// Descripción detallada del evento registrado.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Nombre del usuario asociado a la acción, utilizado para fines de visualización.
        /// </summary>
        public string NombreUsuario { get; set; }
    }
}