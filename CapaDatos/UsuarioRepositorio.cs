using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class UsuarioRepositorio
    {
        Conexion con = new Conexion();

        public void Insertar(Usuario u)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                // *cambio* - Guardamos 'IdRol' en la base de datos en lugar de la cadena de texto de Rol
                string sql = "INSERT INTO Usuarios (NombreUsuario,Password,IdRol,Estado,Salt,IntentosFallidos,FechaBloqueo) " +
                    "VALUES (@NombreUsuario, @Password, @IdRol, @Estado, @Salt, @IntentosFallidos, @FechaBloqueo)";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
                        command.Parameters.AddWithValue("@Password", u.Password);
                        command.Parameters.AddWithValue("@IdRol", u.IdRol); // *cambio*
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

        public void Actualizar(Usuario u)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                // *cambio* - Actualizamos 'IdRol'
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
                        command.Parameters.AddWithValue("@IdRol", u.IdRol); // *cambio*
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

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection conexion = con.Conectar())
            {
                // *cambio* - Traemos el Nombre del Rol haciendo Join con la tabla Roles
                string sql = "SELECT u.*, r.NombreRol FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol";
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
                            u.IdRol = Convert.ToInt32(reader["IdRol"]); // *cambio*
                            u.NombreRol = reader["NombreRol"].ToString(); // *cambio*
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

        public Usuario ObtenerPorId(int idUsuario)
        {
            Usuario u = null;

            using (SqlConnection conexion = con.Conectar())
            {
                // *cambio* - Traemos el Nombre del Rol mediante Join
                string sql = "SELECT u.*, r.NombreRol FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol WHERE u.IdUsuario=@IdUsuario";
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
                            u.IdRol = Convert.ToInt32(reader["IdRol"]); // *cambio*
                            u.NombreRol = reader["NombreRol"].ToString(); // *cambio*
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

        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario u = null;

            using (SqlConnection conexion = con.Conectar())
            {
                // *cambio* - Traemos el Nombre del Rol mediante Join
                string sql = "SELECT u.*, r.NombreRol FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol WHERE u.NombreUsuario=@NombreUsuario";
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
                            u.IdRol = Convert.ToInt32(reader["IdRol"]); // *cambio*
                            u.NombreRol = reader["NombreRol"].ToString(); // *cambio*
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