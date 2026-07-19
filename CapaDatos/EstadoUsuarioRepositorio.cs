using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio responsable de acceder al catálogo de estados de usuario en la base de datos.
    /// En la capa DAL se centraliza la gestión de conexiones, comandos y mapeo de filas a entidades.
    /// </summary>
    public class EstadoUsuarioRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones para la capa de datos. Privada para evitar exponer infraestructura.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Obtiene la lista completa de estados disponibles en el catálogo de la base de datos.
        /// </summary>
        /// <returns>Lista de <see cref="EstadoUsuario"/>. Devuelve una lista vacía si no se encuentran registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve cualquier excepción lanzada durante la operación de acceso a datos.</exception>
        public List<EstadoUsuario> ObtenerTodos()
        {
            List<EstadoUsuario> lista = new List<EstadoUsuario>();
            string query = "SELECT IdEstado, NombreEstado, Descripcion FROM EstadosUsuario";

            try
            {
                // POR QUÉ: Se usa using para asegurar el cierre correcto de la conexión y del comando aunque ocurra una excepción.
                using (SqlConnection conexion = con.Conectar())
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            // Mapeo manual de columnas para controlar conversiones y valores nulos.
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
                // POR QUÉ: se envuelve la excepción para añadir contexto de la capa de datos y preservar el InnerException para diagnóstico.
                throw new Exception("Error en CapaDatos - EstadoUsuarioRepositorio al obtener el catálogo: " + ex.Message, ex);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un estado de usuario por su identificador.
        /// </summary>
        /// <param name="idEstado">Identificador del estado a buscar.</param>
        /// <returns>Instancia de <see cref="EstadoUsuario"/> encontrada, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve cualquier excepción lanzada durante la operación de acceso a datos.</exception>
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
                            // Mapeo manual para controlar nulos y tipos de datos retornados por la consulta.
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
