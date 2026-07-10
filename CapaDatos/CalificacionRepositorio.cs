using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CalificacionRepositorio
    {
        Conexion con = new Conexion();
        //Codigo por Andre Granda 4to C

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
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdMatricula", c.IdMatricula);
                        command.Parameters.AddWithValue("@Nota1", c.Nota1);
                        command.Parameters.AddWithValue("@Nota2", c.Nota2);
                        command.Parameters.AddWithValue("@NotaFinal", c.NotaFinal);
                        command.Parameters.AddWithValue("@NotaMaxima", c.NotaMaxima);
                        command.Parameters.AddWithValue("@Estado", c.Estado);
                        command.Parameters.AddWithValue("@Faltas", c.Faltas);
                        command.Parameters.AddWithValue("@Observaciones", c.Observaciones);
                        command.Parameters.AddWithValue("@FechaCalificacion", c.FechaCalificacion);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar la calificación: " + ex.Message, ex);
                }
            }
        }

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
                        command.Parameters.AddWithValue("@Observaciones", c.Observaciones);
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
                            c.Observaciones = reader["Observaciones"].ToString();
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

        public Calificacion ObtenerPorId(int idCalificacion)
        {
            Calificacion c = null;

            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "SELECT * FROM Calificaciones WHERE IdCalificacion=@IdCalificacion";
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
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
                            c.Observaciones = reader["Observaciones"].ToString();
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