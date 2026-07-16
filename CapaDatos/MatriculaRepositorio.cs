using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Matricula"/>.
    /// </summary>
    public class MatriculaRepositorio
    {
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta una nueva matrícula en la base de datos.
        /// </summary>
        /// <param name="m">Objeto <see cref="Matricula"/> con los datos a insertar.</param>
        public void Insertar(Matricula m)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Matriculas (IdEstudiante,IdAsignatura,IdDocente,IdCurso,IdPeriodo,FechaMatricula,Estado) VALUES (@IdEstudiante, @IdAsignatura, @IdDocente, @IdCurso, @IdPeriodo, @FechaMatricula, @Estado)";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdEstudiante", m.IdEstudiante);
                        command.Parameters.AddWithValue("@IdAsignatura", m.IdAsignatura);
                        command.Parameters.AddWithValue("@IdDocente", m.IdDocente);
                        command.Parameters.AddWithValue("@IdCurso", m.IdCurso);
                        command.Parameters.AddWithValue("@IdPeriodo", m.IdPeriodo);
                        command.Parameters.AddWithValue("@FechaMatricula", m.FechaMatricula);
                        command.Parameters.AddWithValue("@Estado", m.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar la matrícula: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de una matrícula existente.
        /// </summary>
        /// <param name="m">Objeto <see cref="Matricula"/> con los datos actualizados.</param>
        public void Actualizar(Matricula m)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Matriculas SET IdEstudiante=@IdEstudiante, IdAsignatura=@IdAsignatura, IdDocente=@IdDocente, IdCurso=@IdCurso, IdPeriodo=@IdPeriodo, FechaMatricula=@FechaMatricula, Estado=@Estado WHERE IdMatricula=@IdMatricula";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdMatricula", m.IdMatricula);
                        command.Parameters.AddWithValue("@IdEstudiante", m.IdEstudiante);
                        command.Parameters.AddWithValue("@IdAsignatura", m.IdAsignatura);
                        command.Parameters.AddWithValue("@IdDocente", m.IdDocente);
                        command.Parameters.AddWithValue("@IdCurso", m.IdCurso);
                        command.Parameters.AddWithValue("@IdPeriodo", m.IdPeriodo);
                        command.Parameters.AddWithValue("@FechaMatricula", m.FechaMatricula);
                        command.Parameters.AddWithValue("@Estado", m.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar la matrícula: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina una matrícula de la base de datos según su identificador.
        /// </summary>
        /// <param name="idMatricula">Identificador de la matrícula a eliminar.</param>
        public void Eliminar(int idMatricula)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Matriculas WHERE IdMatricula=@IdMatricula";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdMatricula", idMatricula);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar la matrícula: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de matrículas registradas.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Matricula"/>.</returns>
        public List<Matricula> ObtenerTodos()
        {
            List<Matricula> lista = new List<Matricula>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Matriculas";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Matricula m = new Matricula();
                            m.IdMatricula = Convert.ToInt32(reader["IdMatricula"]);
                            m.IdEstudiante = Convert.ToInt32(reader["IdEstudiante"]);
                            m.IdAsignatura = Convert.ToInt32(reader["IdAsignatura"]);
                            m.IdDocente = Convert.ToInt32(reader["IdDocente"]);
                            m.IdCurso = Convert.ToInt32(reader["IdCurso"]);
                            m.IdPeriodo = Convert.ToInt32(reader["IdPeriodo"]);
                            m.FechaMatricula = Convert.ToDateTime(reader["FechaMatricula"]);
                            m.Estado = reader["Estado"].ToString();
                            lista.Add(m);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener las matrículas: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene una matrícula específica según su identificador.
        /// </summary>
        /// <param name="idMatricula">Identificador de la matrícula a buscar.</param>
        /// <returns>Objeto <see cref="Matricula"/> encontrado, o <c>null</c> si no existe.</returns>
        public Matricula ObtenerPorId(int idMatricula)
        {
            Matricula m = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Matriculas WHERE IdMatricula=@IdMatricula";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdMatricula", idMatricula);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            m = new Matricula();
                            m.IdMatricula = Convert.ToInt32(reader["IdMatricula"]);
                            m.IdEstudiante = Convert.ToInt32(reader["IdEstudiante"]);
                            m.IdAsignatura = Convert.ToInt32(reader["IdAsignatura"]);
                            m.IdDocente = Convert.ToInt32(reader["IdDocente"]);
                            m.IdCurso = Convert.ToInt32(reader["IdCurso"]);
                            m.IdPeriodo = Convert.ToInt32(reader["IdPeriodo"]);
                            m.FechaMatricula = Convert.ToDateTime(reader["FechaMatricula"]);
                            m.Estado = reader["Estado"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la matrícula: " + ex.Message, ex);
                }
            }

            return m;
        }
    }
}