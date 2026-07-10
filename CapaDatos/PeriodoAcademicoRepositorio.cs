using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class PeriodoAcademicoRepositorio
    {
        Conexion con = new Conexion();

        public void Insertar(PeriodoAcademico p)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                
                string sql = "INSERT INTO PeriodosAcademicos (NombrePeriodo,FechaInicio,FechaFin,Estado) VALUES (@NombrePeriodo, @FechaInicio, @FechaFin, @Estado)";
                try
                {
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
                    throw new Exception("Error al insertar el periodo académico: " + ex.Message, ex);
                }
            }
        }

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