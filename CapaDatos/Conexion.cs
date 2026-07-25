using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Proporciona la conexión a la base de datos utilizada por todos los repositorios del sistema.
    /// Contiene la cadena de conexión centralizada y la fábrica que crea <see cref="SqlConnection"/>.
    /// </summary>
    public class Conexion
    {
        /// <summary>
        /// Cadena de conexión utilizada por la aplicación. Mantenerla centralizada facilita pruebas y cambios de entorno.
        /// </summary>
        private string cadenaConexion = @"Server=JEAN;Database=AcademicoDB;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Crea y retorna una nueva instancia de conexión SQL configurada con la cadena de conexión del sistema.
        /// </summary>
        /// <returns>Objeto <see cref="SqlConnection"/> listo para ser abierto por el llamador.</returns>
        /// <exception cref="System.ArgumentException">Si la cadena de conexión tiene un formato inválido.</exception>
        /// <exception cref="System.InvalidOperationException">Si ocurre un error al inicializar el proveedor de datos.</exception>
        public SqlConnection Conectar()
        {
            // POR QUÉ: devolvemos una nueva instancia en lugar de una conexión compartida para evitar problemas de concurrencia
            // y que cada operación controle su propio ciclo de vida (Open/Close) con using.
            return new SqlConnection(cadenaConexion);
        }
    }
}
