using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Curso"/>.
    /// </summary>
    public class CursoRepositorio
    {
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo curso en la base de datos.
        /// </summary>
        /// <param name="c">Objeto <see cref="Curso"/> con los datos a insertar.</param>
        public void Insertar(Curso c)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Cursos (NombreCurso,Paralelo,Capacidad,Estado) VALUES (@NombreCurso, @Paralelo, @Capacidad, @Estado)";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@NombreCurso", c.NombreCurso);
                        command.Parameters.AddWithValue("@Paralelo", c.Paralelo);
                        command.Parameters.AddWithValue("@Capacidad", c.Capacidad);
                        command.Parameters.AddWithValue("@Estado", c.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar el curso: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un curso existente.
        /// </summary>
        /// <param name="c">Objeto <see cref="Curso"/> con los datos actualizados.</param>
        public void Actualizar(Curso c)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Cursos SET NombreCurso=@NombreCurso, Paralelo=@Paralelo, Capacidad=@Capacidad, Estado=@Estado WHERE IdCurso=@IdCurso";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCurso", c.IdCurso);
                        command.Parameters.AddWithValue("@NombreCurso", c.NombreCurso);
                        command.Parameters.AddWithValue("@Paralelo", c.Paralelo);
                        command.Parameters.AddWithValue("@Capacidad", c.Capacidad);
                        command.Parameters.AddWithValue("@Estado", c.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar el curso: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina un curso de la base de datos según su identificador.
        /// </summary>
        /// <param name="idCurso">Identificador del curso a eliminar.</param>
        public void Eliminar(int idCurso)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Cursos WHERE IdCurso=@IdCurso";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCurso", idCurso);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar el curso: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de cursos registrados.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Curso"/>.</returns>
        public List<Curso> ObtenerTodos()
        {
            List<Curso> lista = new List<Curso>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Cursos";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Curso c = new Curso();
                            c.IdCurso = Convert.ToInt32(reader["IdCurso"]);
                            c.NombreCurso = reader["NombreCurso"].ToString();
                            c.Paralelo = reader["Paralelo"].ToString();
                            c.Capacidad = Convert.ToInt32(reader["Capacidad"]);
                            c.Estado = reader["Estado"].ToString();
                            lista.Add(c);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los cursos: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un curso específico según su identificador.
        /// </summary>
        /// <param name="idCurso">Identificador del curso a buscar.</param>
        /// <returns>Objeto <see cref="Curso"/> encontrado, o <c>null</c> si no existe.</returns>
        public Curso ObtenerPorId(int idCurso)
        {
            Curso c = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Cursos WHERE IdCurso=@IdCurso";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCurso", idCurso);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            c = new Curso();
                            c.IdCurso = Convert.ToInt32(reader["IdCurso"]);
                            c.NombreCurso = reader["NombreCurso"].ToString();
                            c.Paralelo = reader["Paralelo"].ToString();
                            c.Capacidad = Convert.ToInt32(reader["Capacidad"]);
                            c.Estado = reader["Estado"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el curso: " + ex.Message, ex);
                }
            }

            return c;
        }
    }
}