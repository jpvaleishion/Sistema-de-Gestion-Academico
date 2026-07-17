using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class EstadoUsuarioRepositorio
    {
        private Conexion con = new Conexion();

        /// <summary>
        /// Obtiene la lista completa de estados disponibles en el catálogo de la base de datos.
        /// </summary>
        public List<EstadoUsuario> ObtenerTodos()
        {
            List<EstadoUsuario> lista = new List<EstadoUsuario>();
            string query = "SELECT IdEstado, NombreEstado, Descripcion FROM EstadosUsuario";

            try
            {
                using (SqlConnection conexion = con.Conectar())
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
            catch (Exception ex)
            {
                throw new Exception("Error en CapaDatos - EstadoUsuarioRepositorio al obtener el catálogo: " + ex.Message, ex);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un estado de usuario por su identificador.
        /// </summary>
        /// <param name="idEstado">Identificador del estado.</param>
        /// <returns>Entidad EstadoUsuario correspondiente.</returns>
        public EstadoUsuario ObtenerPorId(int idEstado)
        {
            EstadoUsuario estado = null;
            string query = "SELECT IdEstado, NombreEstado, Descripcion FROM EstadosUsuario WHERE IdEstado = @IdEstado";

            try
            {
                using (SqlConnection conexion = con.Conectar())
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@IdEstado", idEstado);
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            estado = new EstadoUsuario
                            {
                                IdEstado = Convert.ToInt32(lector["IdEstado"]),
                                NombreEstado = lector["NombreEstado"].ToString(),
                                Descripcion = lector["Descripcion"] != DBNull.Value
                                    ? lector["Descripcion"].ToString()
                                    : string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en CapaDatos - EstadoUsuarioRepositorio al obtener por ID: " + ex.Message, ex);
            }

            return estado;
        }
    }
}
