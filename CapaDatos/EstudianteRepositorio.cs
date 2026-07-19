using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Estudiante"/>.
    /// Centraliza la gestión de conexiones, comandos y mapeo de filas a la entidad.
    /// </summary>
    public class EstudianteRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones utilizada por los métodos de la clase.
        /// Mantenerla privada evita exponer detalles de infraestructura a capas superiores.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo estudiante en la base de datos.
        /// </summary>
        /// <param name="e">Objeto <see cref="Estudiante"/> con los datos a insertar. No debe ser null; la validación se realiza en capas superiores.</param>
        /// <returns>Void. Persiste un nuevo registro en la tabla Estudiantes.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) con contexto adicional.</exception>
        public void Insertar(Estudiante e)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Estudiantes (Nombres,Apellidos,Email,Telefono,Estado,CodigoEstudiante,FechaInscripcion,Tipo) VALUES (@Nombres, @Apellidos, @Email, @Telefono, @Estado, @CodigoEstudiante, @FechaInscripcion, @Tipo)";
                try
                {
                    // Abrimos la conexión justo antes de ejecutar el comando para minimizar la ventana en la que el recurso está ocupado.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Uso de parámetros para prevenir inyección SQL y asegurar el correcto tratamiento de tipos.
                        command.Parameters.AddWithValue("@Nombres", e.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", e.Apellidos);
                        command.Parameters.AddWithValue("@Email", e.Email);
                        command.Parameters.AddWithValue("@Telefono", e.Telefono);
                        command.Parameters.AddWithValue("@Estado", e.Estado);
                        command.Parameters.AddWithValue("@CodigoEstudiante", e.CodigoEstudiante);
                        command.Parameters.AddWithValue("@FechaInscripcion", e.FechaInscripcion);
                        command.Parameters.AddWithValue("@Tipo", e.Tipo);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la excepción para aportar contexto de la capa de datos manteniendo la excepción original.
                    throw new Exception("Error al insertar el estudiante: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un estudiante existente.
        /// </summary>
        /// <param name="e">Objeto <see cref="Estudiante"/> con los datos actualizados. Debe contener IdPersona válido.</param>
        /// <returns>Void. Si no existe la fila, la consulta no modificará registros.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Actualizar(Estudiante e)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Estudiantes SET Nombres=@Nombres, Apellidos=@Apellidos, Email=@Email, Telefono=@Telefono, Estado=@Estado, CodigoEstudiante=@CodigoEstudiante, FechaInscripcion=@FechaInscripcion, Tipo=@Tipo WHERE IdPersona=@IdPersona";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPersona", e.IdPersona);
                        command.Parameters.AddWithValue("@Nombres", e.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", e.Apellidos);
                        command.Parameters.AddWithValue("@Email", e.Email);
                        command.Parameters.AddWithValue("@Telefono", e.Telefono);
                        command.Parameters.AddWithValue("@Estado", e.Estado);
                        command.Parameters.AddWithValue("@CodigoEstudiante", e.CodigoEstudiante);
                        command.Parameters.AddWithValue("@FechaInscripcion", e.FechaInscripcion);
                        command.Parameters.AddWithValue("@Tipo", e.Tipo);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar el estudiante: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina un estudiante de la base de datos según su identificador de persona.
        /// </summary>
        /// <param name="idPersona">Identificador de la persona/estudiante a eliminar.</param>
        /// <returns>Void. Operación realiza un DELETE físico sobre la tabla Estudiantes.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Eliminar(int idPersona)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Estudiantes WHERE IdPersona=@IdPersona";
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
                    throw new Exception("Error al eliminar el estudiante: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de estudiantes registrados.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Estudiante"/>. Devuelve una lista vacía si no hay registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public List<Estudiante> ObtenerTodos()
        {
            List<Estudiante> lista = new List<Estudiante>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Estudiantes";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Estudiante e = new Estudiante();
                            // Mapeo explícito para controlar conversiones y valores nulos.
                            e.IdPersona = Convert.ToInt32(reader["IdPersona"]);
                            e.Nombres = reader["Nombres"].ToString();
                            e.Apellidos = reader["Apellidos"].ToString();
                            e.Email = reader["Email"].ToString();
                            e.Telefono = reader["Telefono"].ToString();
                            e.Estado = reader["Estado"].ToString();
                            e.CodigoEstudiante = reader["CodigoEstudiante"].ToString();
                            e.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                            e.Tipo = reader["Tipo"].ToString();
                            lista.Add(e);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los estudiantes: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un estudiante específico según su identificador de persona.
        /// </summary>
        /// <param name="idPersona">Identificador de la persona/estudiante a buscar.</param>
        /// <returns>Objeto <see cref="Estudiante"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public Estudiante ObtenerPorId(int idPersona)
        {
            Estudiante e = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Estudiantes WHERE IdPersona=@IdPersona";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPersona", idPersona);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            e = new Estudiante();
                            // Mapeo manual para asegurar tipos y nullables según contrato de la entidad.
                            e.IdPersona = Convert.ToInt32(reader["IdPersona"]);
                            e.Nombres = reader["Nombres"].ToString();
                            e.Apellidos = reader["Apellidos"].ToString();
                            e.Email = reader["Email"].ToString();
                            e.Telefono = reader["Telefono"].ToString();
                            e.Estado = reader["Estado"].ToString();
                            e.CodigoEstudiante = reader["CodigoEstudiante"].ToString();
                            e.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                            e.Tipo = reader["Tipo"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el estudiante: " + ex.Message, ex);
                }
            }

            return e;
        }
    }
}
