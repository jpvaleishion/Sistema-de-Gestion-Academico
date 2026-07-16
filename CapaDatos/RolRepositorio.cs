using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de consulta de acceso a datos para la entidad <see cref="Rol"/>.
    /// </summary>
    public class RolRepositorio
    {
        private Conexion con = new Conexion();

        /// <summary>
        /// Obtiene la lista completa de roles registrados, ordenados alfabéticamente por nombre.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Rol"/>.</returns>
        public List<Rol> ObtenerTodos()
        {
            List<Rol> lista = new List<Rol>();

            using (SqlConnection conexion = con.Conectar())
            {
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
                                    NombreRol = reader["Nombre"].ToString()
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