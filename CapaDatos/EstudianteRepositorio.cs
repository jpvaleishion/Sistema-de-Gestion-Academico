using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class EstudianteRepositorio
    {
        Conexion con = new Conexion();

        public void Insertar(Estudiante e)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "INSERT INTO Estudiantes (Nombres,Apellidos,Email,Telefono,Estado,CodigoEstudiante,FechaInscripcion,Tipo) VALUES (@Nombres, @Apellidos, @Email, @Telefono, @Estado, @CodigoEstudiante, @FechaInscripcion, @Tipo)";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
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
                    throw new Exception("Error al insertar el estudiante: " + ex.Message, ex);
                }
            }
        }

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