using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Usuario"/>, incluyendo autenticación y control de bloqueo por intentos fallidos.
    /// </summary>
    public class UsuarioRepositorio
    {
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos con sus valores iniciales de seguridad.
        /// </summary>
        /// <param name="u">Objeto <see cref="Usuario"/> con los datos a insertar.</param>
        public void Insertar(Usuario u)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Usuarios (NombreUsuario,Password,IdRol,Estado,Salt,IntentosFallidos,FechaBloqueo) " +
                    "VALUES (@NombreUsuario, @Password, @IdRol, @Estado, @Salt, @IntentosFallidos, @FechaBloqueo)";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
                        command.Parameters.AddWithValue("@Password", u.Password);
                        command.Parameters.AddWithValue("@IdRol", u.IdRol);
                        command.Parameters.AddWithValue("@Estado", u.Estado);
                        command.Parameters.AddWithValue("@Salt", (object)u.Salt ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IntentosFallidos", 0);
                        command.Parameters.AddWithValue("@FechaBloqueo", DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar el usuario: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos generales y de seguridad de un usuario existente.
        /// </summary>
        /// <param name="u">Objeto <see cref="Usuario"/> con los datos actualizados.</param>
        public void Actualizar(Usuario u)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Usuarios SET NombreUsuario=@NombreUsuario, Password=@Password, IdRol=@IdRol, Estado=@Estado, " +
                    "Salt=@Salt, IntentosFallidos=@IntentosFallidos, FechaBloqueo=@FechaBloqueo WHERE IdUsuario=@IdUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
                        command.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
                        command.Parameters.AddWithValue("@Password", u.Password);
                        command.Parameters.AddWithValue("@IdRol", u.IdRol);
                        command.Parameters.AddWithValue("@Estado", u.Estado);
                        command.Parameters.AddWithValue("@Salt", (object)u.Salt ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IntentosFallidos", u.IntentosFallidos);
                        command.Parameters.AddWithValue("@FechaBloqueo", (object)u.FechaBloqueo ?? DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar el usuario: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina un usuario de la base de datos según su identificador.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a eliminar.</param>
        public void Eliminar(int idUsuario)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Usuarios WHERE IdUsuario=@IdUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar el usuario: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios registrados, incluyendo el nombre de su rol asociado.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Usuario"/>.</returns>
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT u.*, r.Nombre AS NombreRol FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Usuario u = new Usuario();
                            u.IdUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            u.NombreUsuario = reader["NombreUsuario"].ToString();
                            u.Password = reader["Password"].ToString();
                            u.IdRol = Convert.ToInt32(reader["IdRol"]);
                            u.NombreRol = reader["NombreRol"].ToString();
                            u.Estado = reader["Estado"].ToString();
                            u.Salt = reader["Salt"] != DBNull.Value ? reader["Salt"].ToString() : null;
                            u.IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]);
                            u.FechaBloqueo = reader["FechaBloqueo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["FechaBloqueo"]) : null;
                            lista.Add(u);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los usuarios: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un usuario específico según su identificador, incluyendo el nombre de su rol asociado.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a buscar.</param>
        /// <returns>Objeto <see cref="Usuario"/> encontrado, o <c>null</c> si no existe.</returns>
        public Usuario ObtenerPorId(int idUsuario)
        {
            Usuario u = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT u.*, r.Nombre AS NombreRol FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol WHERE u.IdUsuario=@IdUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            u = new Usuario();
                            u.IdUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            u.NombreUsuario = reader["NombreUsuario"].ToString();
                            u.Password = reader["Password"].ToString();
                            u.IdRol = Convert.ToInt32(reader["IdRol"]);
                            u.NombreRol = reader["NombreRol"].ToString();
                            u.Estado = reader["Estado"].ToString();
                            u.Salt = reader["Salt"] != DBNull.Value ? reader["Salt"].ToString() : null;
                            u.IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]);
                            u.FechaBloqueo = reader["FechaBloqueo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["FechaBloqueo"]) : null;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el usuario: " + ex.Message, ex);
                }
            }

            return u;
        }

        /// <summary>
        /// Obtiene un usuario específico según su nombre de usuario, incluyendo el nombre de su rol asociado.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a buscar.</param>
        /// <returns>Objeto <see cref="Usuario"/> encontrado, o <c>null</c> si no existe.</returns>
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario u = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT u.*, r.Nombre AS NombreRol FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol WHERE u.NombreUsuario=@NombreUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            u = new Usuario();
                            u.IdUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            u.NombreUsuario = reader["NombreUsuario"].ToString();
                            u.Password = reader["Password"].ToString();
                            u.IdRol = Convert.ToInt32(reader["IdRol"]);
                            u.NombreRol = reader["NombreRol"].ToString();
                            u.Estado = reader["Estado"].ToString();
                            u.Salt = reader["Salt"] != DBNull.Value ? reader["Salt"].ToString() : null;
                            u.IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]);
                            u.FechaBloqueo = reader["FechaBloqueo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["FechaBloqueo"]) : null;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el usuario por nombre de usuario: " + ex.Message, ex);
                }
            }

            return u;
        }

        /// <summary>
        /// Actualiza el número de intentos fallidos de inicio de sesión y la fecha de bloqueo de un usuario.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a actualizar.</param>
        /// <param name="intentos">Nuevo número de intentos fallidos registrados.</param>
        /// <param name="fechaBloqueo">Nueva fecha de bloqueo, o <c>null</c> si el usuario no está bloqueado.</param>
        public void ActualizarIntentosYBloqueo(int idUsuario, int intentos, DateTime? fechaBloqueo)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Usuarios SET IntentosFallidos = @Intentos, FechaBloqueo = @Fecha WHERE IdUsuario = @Id";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@Id", idUsuario);
                        command.Parameters.AddWithValue("@Intentos", intentos);
                        command.Parameters.AddWithValue("@Fecha", fechaBloqueo.HasValue ? (object)fechaBloqueo.Value : DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar los intentos y el bloqueo del usuario: " + ex.Message, ex);
                }
            }
        }
    }
}