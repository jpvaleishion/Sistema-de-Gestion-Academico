using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Calificacion"/>.
    /// </summary>
    public class CalificacionRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones utilizada por los métodos de la clase.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta una nueva calificación en la base de datos.
        /// </summary>
        /// <param name="c">Objeto <see cref="Calificacion"/> con los datos a insertar.</param>
        /// <returns>Void. Persiste la calificación en la tabla correspondiente.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Insertar(Calificacion c)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Calificaciones (IdMatricula,Nota1," +
                    "Nota2,NotaFinal,NotaMaxima,Estado,Faltas,Observaciones,FechaCalificacion) " +
                    "VALUES (@IdMatricula, @Nota1, @Nota2, @NotaFinal, @NotaMaxima, @Estado, @Faltas, " +
                    "@Observaciones, @FechaCalificacion)";
                try
                {
                    // Abrimos la conexión lo más tarde posible para minimizar la ventana en la que está abierta.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Uso de parámetros para prevenir inyección SQL y garantizar conversiones correctas.
                        command.Parameters.AddWithValue("@IdMatricula", c.IdMatricula);
                        command.Parameters.AddWithValue("@Nota1", c.Nota1);
                        command.Parameters.AddWithValue("@Nota2", c.Nota2);
                        command.Parameters.AddWithValue("@NotaFinal", c.NotaFinal);
                        command.Parameters.AddWithValue("@NotaMaxima", c.NotaMaxima);
                        command.Parameters.AddWithValue("@Estado", c.Estado);
                        command.Parameters.AddWithValue("@Faltas", c.Faltas);
                        command.Parameters.AddWithValue("@Observaciones", c.Observaciones ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FechaCalificacion", c.FechaCalificacion);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la excepción para aportar contexto desde la DAL y conservar InnerException.
                    throw new Exception("Error al insertar la calificación: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de una calificación existente.
        /// </summary>
        /// <param name="c">Objeto <see cref="Calificacion"/> con los datos actualizados.</param>
        /// <returns>Void. Ejecuta la actualización sobre la fila identificada por IdCalificacion.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Actualizar(Calificacion c)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Calificaciones SET IdMatricula=@IdMatricula, Nota1=@Nota1, Nota2=@Nota2, NotaFinal=@NotaFinal, NotaMaxima=@NotaMaxima, " +
                    "Estado=@Estado, Faltas=@Faltas, Observaciones=@Observaciones, " +
                    "FechaCalificacion=@FechaCalificacion WHERE IdCalificacion=@IdCalificacion";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCalificacion", c.IdCalificacion);
                        command.Parameters.AddWithValue("@IdMatricula", c.IdMatricula);
                        command.Parameters.AddWithValue("@Nota1", c.Nota1);
                        command.Parameters.AddWithValue("@Nota2", c.Nota2);
                        command.Parameters.AddWithValue("@NotaFinal", c.NotaFinal);
                        command.Parameters.AddWithValue("@NotaMaxima", c.NotaMaxima);
                        command.Parameters.AddWithValue("@Estado", c.Estado);
                        command.Parameters.AddWithValue("@Faltas", c.Faltas);
                        command.Parameters.AddWithValue("@Observaciones", c.Observaciones ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FechaCalificacion", c.FechaCalificacion);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar la calificación: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina una calificación de la base de datos según su identificador.
        /// </summary>
        /// <param name="idCalificacion">Identificador de la calificación a eliminar.</param>
        /// <returns>Void.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Eliminar(int idCalificacion)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Calificaciones WHERE IdCalificacion=@IdCalificacion";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCalificacion", idCalificacion);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar la calificación: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de calificaciones registradas.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Calificacion"/>. Devuelve lista vacía si no hay registros.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public List<Calificacion> ObtenerTodos()
        {
            List<Calificacion> lista = new List<Calificacion>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Calificaciones";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Calificacion c = new Calificacion();
                            c.IdCalificacion = Convert.ToInt32(reader["IdCalificacion"]);
                            c.IdMatricula = Convert.ToInt32(reader["IdMatricula"]);
                            c.Nota1 = Convert.ToDecimal(reader["Nota1"]);
                            c.Nota2 = Convert.ToDecimal(reader["Nota2"]);
                            c.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                            c.NotaMaxima = Convert.ToDecimal(reader["NotaMaxima"]);
                            c.Estado = reader["Estado"].ToString();
                            c.Faltas = Convert.ToInt32(reader["Faltas"]);
                            c.Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null;
                            c.FechaCalificacion = Convert.ToDateTime(reader["FechaCalificacion"]);
                            lista.Add(c);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener las calificaciones: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene una calificación específica según su identificador.
        /// </summary>
        /// <param name="idCalificacion">Identificador de la calificación a buscar.</param>
        /// <returns>Objeto <see cref="Calificacion"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public Calificacion ObtenerPorId(int idCalificacion)
        {
            Calificacion c = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Calificaciones WHERE IdCalificacion=@IdCalificacion";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCalificacion", idCalificacion);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            c = new Calificacion();
                            c.IdCalificacion = Convert.ToInt32(reader["IdCalificacion"]);
                            c.IdMatricula = Convert.ToInt32(reader["IdMatricula"]);
                            c.Nota1 = Convert.ToDecimal(reader["Nota1"]);
                            c.Nota2 = Convert.ToDecimal(reader["Nota2"]);
                            c.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                            c.NotaMaxima = Convert.ToDecimal(reader["NotaMaxima"]);
                            c.Estado = reader["Estado"].ToString();
                            c.Faltas = Convert.ToInt32(reader["Faltas"]);
                            c.Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null;
                            c.FechaCalificacion = Convert.ToDateTime(reader["FechaCalificacion"]);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la calificación: " + ex.Message, ex);
                }
            }

            return c;
        }
    }
}
