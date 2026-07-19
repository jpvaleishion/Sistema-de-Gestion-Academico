using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Docente"/>.
    /// Implementa operaciones CRUD y mapeo manual entre filas de la base de datos y la entidad <see cref="Docente"/>.
    /// </summary>
    public class DocenteRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones compartida en la capa de datos.
        /// Mantenerla privada evita fugas de dependencias a capas superiores.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo docente en la base de datos.
        /// </summary>
        /// <param name="d">Objeto <see cref="Docente"/> con los datos a insertar. No debe ser null.</param>
        /// <returns>Void. Persiste el registro en la tabla Docentes.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) con contexto adicional.</exception>
        public void Insertar(Docente d)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Docentes (Nombres,Apellidos,Email,Telefono,Estado,Especialidad) VALUES (@Nombres, @Apellidos, @Email, @Telefono, @Estado, @Especialidad)";
                try
                {
                    // Abrimos la conexión justo antes de ejecutar la operación para reducir el tiempo de ocupación del recurso.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Uso de parámetros para prevenir inyección SQL y asegurar mapeo de tipos correcto.
                        command.Parameters.AddWithValue("@Nombres", d.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", d.Apellidos);
                        command.Parameters.AddWithValue("@Email", d.Email);
                        command.Parameters.AddWithValue("@Telefono", d.Telefono);
                        command.Parameters.AddWithValue("@Estado", d.Estado);
                        command.Parameters.AddWithValue("@Especialidad", d.Especialidad);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la excepción para proporcionar contexto de la capa de datos manteniendo el InnerException.
                    throw new Exception("Error al insertar el docente: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un docente existente.
        /// </summary>
        /// <param name="d">Objeto <see cref="Docente"/> con los datos actualizados. Debe contener IdPersona válido.</param>
        /// <returns>Void. Si no existe la fila no se lanzará excepción; la consulta no modificará registros.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Actualizar(Docente d)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Docentes SET Nombres=@Nombres, Apellidos=@Apellidos, Email=@Email, Telefono=@Telefono, Estado=@Estado, Especialidad=@Especialidad WHERE IdPersona=@IdPersona";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPersona", d.IdPersona);
                        command.Parameters.AddWithValue("@Nombres", d.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", d.Apellidos);
                        command.Parameters.AddWithValue("@Email", d.Email);
                        command.Parameters.AddWithValue("@Telefono", d.Telefono);
                        command.Parameters.AddWithValue("@Estado", d.Estado);
                        command.Parameters.AddWithValue("@Especialidad", d.Especialidad);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar el docente: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina un docente de la base de datos según su identificador de persona.
        /// </summary>
        /// <param name="idPersona">Identificador de la persona/docente a eliminar.</param>
        /// <returns>Void. Realiza un DELETE físico en la tabla Docentes.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Eliminar(int idPersona)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Docentes WHERE IdPersona=@IdPersona";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPersona", idPersona);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar el docente: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de docentes registrados.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Docente"/>. Devuelve una lista vacía si no hay registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public List<Docente> ObtenerTodos()
        {
            List<Docente> lista = new List<Docente>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Docentes";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Docente d = new Docente();
                            d.IdPersona = Convert.ToInt32(reader["IdPersona"]);
                            d.Nombres = reader["Nombres"].ToString();
                            d.Apellidos = reader["Apellidos"].ToString();
                            d.Email = reader["Email"].ToString();
                            d.Telefono = reader["Telefono"].ToString();
                            d.Estado = reader["Estado"].ToString();
                            d.Especialidad = reader["Especialidad"].ToString();
                            lista.Add(d);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los docentes: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un docente específico según su identificador de persona.
        /// </summary>
        /// <param name="idPersona">Identificador de la persona/docente a buscar.</param>
        /// <returns>Objeto <see cref="Docente"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public Docente ObtenerPorId(int idPersona)
        {
            Docente d = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Docentes WHERE IdPersona=@IdPersona";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPersona", idPersona);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            d = new Docente();
                            d.IdPersona = Convert.ToInt32(reader["IdPersona"]);
                            d.Nombres = reader["Nombres"].ToString();
                            d.Apellidos = reader["Apellidos"].ToString();
                            d.Email = reader["Email"].ToString();
                            d.Telefono = reader["Telefono"].ToString();
                            d.Estado = reader["Estado"].ToString();
                            d.Especialidad = reader["Especialidad"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el docente: " + ex.Message, ex);
                }
            }

            return d;
        }
    }
}
