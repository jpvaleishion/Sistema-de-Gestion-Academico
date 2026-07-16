using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class EstadoUsuarioRepositorio
    {
        private Conexion con= new Conexion();

        /// <summary>
        /// Obtiene la lista completa de estados disponibles en el catálogo de la base de datos.
        /// </summary>
        /// <returns>Una lista de objetos EstadoUsuario.</returns>
        public List<EstadoUsuario> ObtenerTodos()
        {
            List<EstadoUsuario> lista = new List<EstadoUsuario>();
            string query = "SELECT IdEstado, NombreEstado, Descripcion FROM EstadosUsuario";

            try
            {
                using (SqlConnection conexion = con.Conectar())
                {
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        conexion.Open();

                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                lista.Add(new EstadoUsuario
                                {
                                    IdEstado = Convert.ToInt32(lector["IdEstado"]),
                                    NombreEstado = lector["NombreEstado"].ToString(),
                                    Descripcion = lector["Descripcion"] != DBNull.Value
                                        ? lector["Descripcion"].ToString()
                                        : string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Propagamos el error con un mensaje claro para capturarlo en las capas superiores
                throw new Exception("Error en CapaDatos - EstadoUsuarioRepositorio al obtener el catálogo: " + ex.Message, ex);
            }

            return lista;
        }
    }
}