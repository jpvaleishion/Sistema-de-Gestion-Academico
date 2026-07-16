using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Proporciona la conexión a la base de datos utilizada por todos los repositorios del sistema.
    /// </summary>
    public class Conexion
    {
        private string cadenaConexion = @"Server=JEAN;Database=AcademicoDB;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Crea y retorna una nueva instancia de conexión SQL configurada con la cadena de conexión del sistema.
        /// </summary>
        /// <returns>Objeto <see cref="SqlConnection"/> listo para ser abierto.</returns>
        public SqlConnection Conectar()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}