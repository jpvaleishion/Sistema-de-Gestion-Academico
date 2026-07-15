using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class RolRepositorio
    {
        private Conexion con = new Conexion();

        public List<Rol> ObtenerTodos()
        {
            List<Rol> lista = new List<Rol>();

            using (SqlConnection conexion = con.Conectar())
            {
                // *cambio* - Cambiamos 'NombreRol' por 'Nombre' en la consulta SQL
                string sql = "SELECT IdRol, Nombre FROM Roles ORDER BY Nombre ASC";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Rol r = new Rol
                                {
                                    IdRol = Convert.ToInt32(reader["IdRol"]),
                                    NombreRol = reader["Nombre"].ToString() // *cambio* - Leemos la columna 'Nombre'
                                };
                                lista.Add(r);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la lista de roles desde la base de datos: " + ex.Message, ex);
                }
            }

            return lista;
        }
    }
}