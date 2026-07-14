using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class RolRepositorio
    {
        private Conexion con = new Conexion();

        // Método para obtener todos los roles activos/disponibles para el ComboBox
        public List<Rol> ObtenerTodos()
        {
            List<Rol> lista = new List<Rol>();

            using (SqlConnection conexion = con.Conectar())
            {
                // Consulta simple para traer los roles de la base de datos
                string sql = "SELECT IdRol, NombreRol FROM Roles ORDER BY NombreRol ASC";
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
                                    NombreRol = reader["NombreRol"].ToString()
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