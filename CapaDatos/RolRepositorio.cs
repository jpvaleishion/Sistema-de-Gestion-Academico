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
        /// <summary>
        /// Fábrica de conexiones de la capa de datos.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Obtiene la lista completa de roles registrados, ordenados alfabéticamente por nombre.
        /// </summary>
        /// <returns>Lista de <see cref="Rol"/>. Devuelve lista vacía si no hay registros.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL con contexto de la DAL.</exception>
        public List<Rol> ObtenerTodos()
        {
            List<Rol> lista = new List<Rol>();
            string sql = "SELECT IdRol, Nombre FROM Roles ORDER BY Nombre ASC";

            using (SqlConnection conexion = con.Conectar())
            {
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Mapeo explícito para evitar dependencia en el orden de columnas.
                            Rol r = new Rol
                            {
                                IdRol = Convert.ToInt32(reader["IdRol"]),
                                NombreRol = reader["Nombre"].ToString()
                            };
                            lista.Add(r);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: se encapsula la excepción para mantener la responsabilidad de la DAL en el manejo de errores.
                    throw new Exception("Error al obtener la lista de roles desde la base de datos: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un rol por su identificador.
        /// </summary>
        /// <param name="idRol">Identificador del rol.</param>
        /// <returns>Entidad Rol correspondiente o null si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL con contexto de la DAL.</exception>
        public Rol ObtenerPorId(int idRol)
        {
            Rol rol = null;
            string sql = "SELECT IdRol, Nombre FROM Roles WHERE IdRol = @IdRol";

            using (SqlConnection conexion = con.Conectar())
            {
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdRol", idRol);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rol = new Rol
                                {
                                    IdRol = Convert.ToInt32(reader["IdRol"]),
                                    NombreRol = reader["Nombre"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el rol desde la base de datos: " + ex.Message, ex);
                }
            }

            return rol;
        }
    }
}
