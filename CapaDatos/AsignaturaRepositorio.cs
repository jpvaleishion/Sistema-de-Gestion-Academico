using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class AsignaturaRepositorio
    {
        Conexion con = new Conexion();

        public void Insertar(Asignatura a)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "INSERT INTO Asignaturas (Nombre,Creditos,Modalidad,Estado) VALUES (@Nombre, @Creditos, @Modalidad, @Estado)";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@Nombre", a.Nombre);
                        command.Parameters.AddWithValue("@Creditos", a.Creditos);
                        command.Parameters.AddWithValue("@Modalidad", a.Modalidad);
                        command.Parameters.AddWithValue("@Estado", a.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar la asignatura: " + ex.Message, ex);
                }
            }
        }

        public void Actualizar(Asignatura a)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "UPDATE Asignaturas SET Nombre=@Nombre, Creditos=@Creditos, Modalidad=@Modalidad, Estado=@Estado WHERE IdAsignatura=@IdAsignatura";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdAsignatura", a.IdAsignatura);
                        command.Parameters.AddWithValue("@Nombre", a.Nombre);
                        command.Parameters.AddWithValue("@Creditos", a.Creditos);
                        command.Parameters.AddWithValue("@Modalidad", a.Modalidad);
                        command.Parameters.AddWithValue("@Estado", a.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar la asignatura: " + ex.Message, ex);
                }
            }
        }

        public void Eliminar(int idAsignatura)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Asignaturas WHERE IdAsignatura=@IdAsignatura";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdAsignatura", idAsignatura);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar la asignatura: " + ex.Message, ex);
                }
            }
        }

        public List<Asignatura> ObtenerTodos()
        {
            List<Asignatura> lista = new List<Asignatura>();

            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "SELECT * FROM Asignaturas";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Asignatura a = new Asignatura();
                            a.IdAsignatura = Convert.ToInt32(reader["IdAsignatura"]);
                            a.Nombre = reader["Nombre"].ToString();
                            a.Creditos = Convert.ToInt32(reader["Creditos"]);
                            a.Modalidad = reader["Modalidad"].ToString();
                            a.Estado = reader["Estado"].ToString();
                            lista.Add(a);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener las asignaturas: " + ex.Message, ex);
                }
            }

            return lista;
        }

        public Asignatura ObtenerPorId(int idAsignatura)
        {
            Asignatura a = null;

            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "SELECT * FROM Asignaturas WHERE IdAsignatura=@IdAsignatura";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdAsignatura", idAsignatura);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            a = new Asignatura();
                            a.IdAsignatura = Convert.ToInt32(reader["IdAsignatura"]);
                            a.Nombre = reader["Nombre"].ToString();
                            a.Creditos = Convert.ToInt32(reader["Creditos"]);
                            a.Modalidad = reader["Modalidad"].ToString();
                            a.Estado = reader["Estado"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la asignatura: " + ex.Message, ex);
                }
            }

            return a;
        }
    }
}