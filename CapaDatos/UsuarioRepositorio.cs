using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Usuario"/>, 
    /// incluyendo autenticación, borrado lógico y control de bloqueo por intentos fallidos.
    /// </summary>
    public class UsuarioRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones usada por la capa de datos.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos con sus valores iniciales de seguridad y estado.
        /// </summary>
        /// <param name="u">Objeto <see cref="Usuario"/> con los datos a insertar.</param>
        /// <returns>Void. Persiste un nuevo usuario en la tabla Usuarios.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) con contexto adicional.</exception>
        public void Insertar(Usuario u)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Usuarios (NombreUsuario, Password, IdRol, IdEstado, MotivoEstado, Salt, IntentosFallidos, FechaBloqueo) " +
                             "VALUES (@NombreUsuario, @Password, @IdRol, @IdEstado, @MotivoEstado, @Salt, @IntentosFallidos, @FechaBloqueo)";
                try
                {
                    // Abrimos la conexión lo más tarde posible para reducir la ventana de ocupación del recurso.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
                        command.Parameters.AddWithValue("@Password", u.Password);
                        command.Parameters.AddWithValue("@IdRol", u.IdRol);
                        command.Parameters.AddWithValue("@IdEstado", u.IdEstado);
                        command.Parameters.AddWithValue("@MotivoEstado", (object)u.MotivoEstado ?? DBNull.Value);
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
        /// Actualiza los datos generales, de seguridad y el estado de un usuario existente.
        /// </summary>
        /// <param name="u">Objeto <see cref="Usuario"/> con los datos actualizados.</param>
        /// <returns>Void. Ejecuta la actualización en la fila identificada por IdUsuario.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Actualizar(Usuario u)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Usuarios SET NombreUsuario=@NombreUsuario, Password=@Password, IdRol=@IdRol, " +
                             "IdEstado=@IdEstado, MotivoEstado=@MotivoEstado, Salt=@Salt, " +
                             "IntentosFallidos=@IntentosFallidos, FechaBloqueo=@FechaBloqueo WHERE IdUsuario=@IdUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
                        command.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
                        command.Parameters.AddWithValue("@Password", u.Password);
                        command.Parameters.AddWithValue("@IdRol", u.IdRol);
                        command.Parameters.AddWithValue("@IdEstado", u.IdEstado);
                        command.Parameters.AddWithValue("@MotivoEstado", (object)u.MotivoEstado ?? DBNull.Value);
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
        /// Realiza un borrado lógico del usuario en el sistema. 
        /// En lugar de un DELETE físico, actualiza el estado a Baneado (IdEstado = 3) y registra el motivo correspondiente.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a eliminar lógicamente.</param>
        public void Eliminar(int idUsuario)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                // Regla de Oro: Borrado Lógico (IdEstado = 3 representa 'Baneado/Eliminado')
                string sql = "UPDATE Usuarios SET IdEstado = 3, " +
                             "MotivoEstado = 'Usuario eliminado lógicamente del sistema por el administrador' " +
                             "WHERE IdUsuario = @IdUsuario";
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
                    throw new Exception("Error al realizar el borrado lógico del usuario: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios activos o inactivos, omitiendo aquellos que fueron borrados lógicamente.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Usuario"/>.</returns>
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection conexion = con.Conectar())
            {
                // Unimos la tabla Usuarios con Roles y EstadosUsuario, excluyendo a los baneados/eliminados (u.IdEstado != 3)
                string sql = "SELECT u.*, r.Nombre AS NombreRol, e.NombreEstado " +
                             "FROM Usuarios u " +
                             "INNER JOIN Roles r ON u.IdRol = r.IdRol " +
                             "INNER JOIN EstadosUsuario e ON u.IdEstado = e.IdEstado " +
                             "WHERE u.IdEstado != 3";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuario u = MapearUsuario(reader);
                                lista.Add(u);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la lista de usuarios: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un usuario específico según su identificador, incluyendo su rol y estado.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a buscar.</param>
        /// <returns>Objeto <see cref="Usuario"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <summary>
        /// Obtiene un usuario específico según su identificador, incluyendo su rol y estado.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a buscar.</param>
        /// <returns>Objeto <see cref="Usuario"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException).</exception>
        public Usuario ObtenerPorId(int idUsuario)
        {
            Usuario u = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT u.*, r.Nombre AS NombreRol, e.NombreEstado " +
                             "FROM Usuarios u " +
                             "INNER JOIN Roles r ON u.IdRol = r.IdRol " +
                             "INNER JOIN EstadosUsuario e ON u.IdEstado = e.IdEstado " +
                             "WHERE u.IdUsuario = @IdUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                u = MapearUsuario(reader);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el usuario por ID: " + ex.Message, ex);
                }
            }

            return u;
        }

        /// <summary>
        /// Obtiene un usuario específico según su nombre de usuario, útil para el inicio de sesión y validaciones.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a buscar.</param>
        /// <returns>Objeto <see cref="Usuario"/> encontrado, o <c>null</c> si no existe.</returns>
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario u = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT u.*, r.Nombre AS NombreRol, e.NombreEstado " +
                             "FROM Usuarios u " +
                             "INNER JOIN Roles r ON u.IdRol = r.IdRol " +
                             "INNER JOIN EstadosUsuario e ON u.IdEstado = e.IdEstado " +
                             "WHERE u.NombreUsuario = @NombreUsuario";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                u = MapearUsuario(reader);
                            }
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
        /// <returns>Void. Actualiza los campos IntentosFallidos y FechaBloqueo en la fila del usuario.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
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

        /// <summary>
        /// Método auxiliar privado para centralizar y unificar el mapeo de la base de datos a la entidad Usuario.
        /// </summary>
        /// <param name="reader">SqlDataReader posicionado en una fila válida de resultado.</param>
        /// <returns>Instancia de <see cref="Usuario"/> poblada con los campos leídos de la fila.</returns>
        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                NombreUsuario = reader["NombreUsuario"].ToString(),
                Password = reader["Password"].ToString(),
                IdRol = Convert.ToInt32(reader["IdRol"]),
                NombreRol = reader["NombreRol"].ToString(),
                IdEstado = Convert.ToInt32(reader["IdEstado"]),
                NombreEstado = reader["NombreEstado"].ToString(),
                MotivoEstado = reader["MotivoEstado"] != DBNull.Value ? reader["MotivoEstado"].ToString() : null,
                Salt = reader["Salt"] != DBNull.Value ? reader["Salt"].ToString() : null,
                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                FechaBloqueo = reader["FechaBloqueo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["FechaBloqueo"]) : null
            };
        }
    }
}
