using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class BitacoraRepositorio
    {
        Conexion con = new Conexion();

        public void Insertar(Bitacora b)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand("sp_InsertarBitacora", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdUsuario", b.IdUsuario);
                        command.Parameters.AddWithValue("@Modulo", b.Modulo);
                        command.Parameters.AddWithValue("@Accion", b.Accion);
                        command.Parameters.AddWithValue("@Descripcion", (object)b.Descripcion ?? DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar el registro de bitácora: " + ex.Message, ex);
                }
            }
        }

        public List<Bitacora> ObtenerTodo()
        {
            List<Bitacora> lista = new List<Bitacora>();

            using (SqlConnection conexion = con.Conectar())
            {
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand("sp_ObtenerBitacora", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Bitacora b = new Bitacora();
                            b.IdBitacora = Convert.ToInt32(reader["IdBitacora"]);
                            b.IdUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            b.NombreUsuario = reader["NombreUsuario"].ToString();
                            b.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                            b.Modulo = reader["Modulo"].ToString();
                            b.Accion = reader["Accion"].ToString();
                            b.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null;
                            lista.Add(b);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la bitácora: " + ex.Message, ex);
                }
            }

            return lista;
        }
    }
}