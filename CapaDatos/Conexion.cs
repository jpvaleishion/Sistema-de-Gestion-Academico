using System.Data.SqlClient;

namespace CapaDatos
{
    public class Conexion
    {
        private string cadenaConexion = @"Server=JEAN;Database=AcademicoDB;Integrated Security=True;TrustServerCertificate=True;";

        public SqlConnection Conectar()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}