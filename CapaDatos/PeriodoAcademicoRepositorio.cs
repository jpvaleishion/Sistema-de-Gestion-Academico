using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="PeriodoAcademico"/>.
    /// Centraliza operaciones CRUD y mapeo entre filas de la base de datos y la entidad.
    /// </summary>
    public class PeriodoAcademicoRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones utilizada por los métodos de la clase.
        /// Mantenerla privada evita exponer detalles de infraestructura a capas superiores.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo periodo académico en la base de datos.
        /// </summary>
        /// <param name="p">Objeto <see cref="PeriodoAcademico"/> con los datos a insertar. No debe ser null.</param>
        /// <returns>Void. Persiste el periodo en la tabla PeriodosAcademicos.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) con contexto adicional.</exception>
        public void Insertar(PeriodoAcademico p)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO PeriodosAcademicos (NombrePeriodo,FechaInicio,FechaFin,Estado) VALUES (@NombrePeriodo, @FechaInicio, @FechaFin, @Estado)";
                try
                {
                    // Abrimos la conexión justo antes de ejecutar el comando para minimizar la ventana en la que el recurso está ocupado.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombrePeriodo", p.NombrePeriodo);
                        command.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", p.FechaFin);
                        command.Parameters.AddWithValue("@Estado", p.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la excepción para proporcionar contexto de la DAL y mantener la InnerException para diagnóstico.
                    throw new Exception("Error al insertar el periodo académico: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un periodo académico existente.
        /// </summary>
        /// <param name="p">Objeto <see cref="PeriodoAcademico"/> con los datos actualizados. Debe contener IdPeriodo válido.</param>
        /// <returns>Void. Si la fila no existe, la consulta no modificará registros.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Actualizar(PeriodoAcademico p)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE PeriodosAcademicos SET NombrePeriodo=@NombrePeriodo, FechaInicio=@FechaInicio, FechaFin=@FechaFin, Estado=@Estado WHERE IdPeriodo=@IdPeriodo";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPeriodo", p.IdPeriodo);
                        command.Parameters.AddWithValue("@NombrePeriodo", p.NombrePeriodo);
                        command.Parameters.AddWithValue("@FechaInicio", p.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", p.FechaFin);
                        command.Parameters.AddWithValue("@Estado", p.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar el periodo académico: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina un periodo académico de la base de datos según su identificador.
        /// </summary>
        /// <param name="idPeriodo">Identificador del periodo académico a eliminar.</param>
        /// <returns>Void. Operación realiza un DELETE físico sobre la tabla PeriodosAcademicos.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Eliminar(int idPeriodo)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM PeriodosAcademicos WHERE IdPeriodo=@IdPeriodo";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPeriodo", idPeriodo);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar el periodo académico: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de periodos académicos registrados.
        /// </summary>
        /// <returns>Lista de objetos <see cref="PeriodoAcademico"/>. Devuelve una lista vacía si no hay registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public List<PeriodoAcademico> ObtenerTodos()
        {
            List<PeriodoAcademico> lista = new List<PeriodoAcademico>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM PeriodosAcademicos";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            PeriodoAcademico p = new PeriodoAcademico();
                            // Mapeo explícito de columnas para controlar conversiones y nullables.
                            p.IdPeriodo = Convert.ToInt32(reader["IdPeriodo"]);
                            p.NombrePeriodo = reader["NombrePeriodo"].ToString();
                            p.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
                            p.FechaFin = Convert.ToDateTime(reader["FechaFin"]);
                            p.Estado = reader["Estado"].ToString();
                            lista.Add(p);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los periodos académicos: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un periodo académico específico según su identificador.
        /// </summary>
        /// <param name="idPeriodo">Identificador del periodo académico a buscar.</param>
        /// <returns>Objeto <see cref="PeriodoAcademico"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public PeriodoAcademico ObtenerPorId(int idPeriodo)
        {
            PeriodoAcademico p = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM PeriodosAcademicos WHERE IdPeriodo=@IdPeriodo";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdPeriodo", idPeriodo);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            p = new PeriodoAcademico();
                            // Mapeo manual para asegurar tipos y nullables según contrato de la entidad.
                            p.IdPeriodo = Convert.ToInt32(reader["IdPeriodo"]);
                            p.NombrePeriodo = reader["NombrePeriodo"].ToString();
                            p.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
                            p.FechaFin = Convert.ToDateTime(reader["FechaFin"]);
                            p.Estado = reader["Estado"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el periodo académico: " + ex.Message, ex);
                }
            }

            return p;
        }
    }
}
