using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar el registro y la consulta de eventos de auditoría del sistema.
    /// Implementa operaciones para insertar entradas de auditoría y recuperar el historial mediante procedimientos almacenados.
    /// </summary>
    public class BitacoraRepositorio
    {
        /// <summary>
        /// Fábrica de conexiones centralizada para la capa de datos.
        /// Se mantiene privada para evitar exponer detalles de infraestructura fuera de la capa de datos.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo registro de bitácora mediante el procedimiento almacenado correspondiente.
        /// </summary>
        /// <param name="b">Objeto <see cref="Bitacora"/> con los datos del evento a registrar. No debe ser null.</param>
        /// <returns>Void. Persiste el evento en la bitácora de la base de datos.</returns>
        /// <exception cref="System.ArgumentNullException">Si <paramref name="b"/> es null (no comprobado internamente; el llamador debe validar).</exception>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) con contexto adicional para la capa superior.</exception>
        public void Insertar(Bitacora b)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                try
                {
                    // Abrimos la conexión lo más tarde posible para minimizar la ventana en la que la conexión permanece abierta.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand("sp_InsertarBitacora", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Uso de parámetros para prevenir inyección SQL y permitir que el motor de datos trate correctamente los tipos.
                        command.Parameters.AddWithValue("@IdUsuario", b.IdUsuario);
                        command.Parameters.AddWithValue("@Modulo", b.Modulo);
                        command.Parameters.AddWithValue("@Accion", b.Accion);

                        // Se usa DBNull.Value cuando la descripción es null para que el SP reciba un valor SQL nulo.
                        // POR QUÉ: distinguir entre cadena vacía y NULL puede ser relevante para reportes/auditoría en BD.
                        command.Parameters.AddWithValue("@Descripcion", (object)b.Descripcion ?? DBNull.Value);

                        // Ejecutamos el procedimiento. No se captura el número de filas afectadas porque el contrato es simplemente registrar el evento.
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la SqlException para añadir contexto desde la capa de datos sin perder la excepción original como InnerException.
                    throw new Exception("Error al insertar el registro de bitácora: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los registros de bitácora mediante el procedimiento almacenado correspondiente.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Bitacora"/>. Devuelve una lista vacía si no existen registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) con contexto adicional.</exception>
        public List<Bitacora> ObtenerTodo()
        {
            List<Bitacora> lista = new List<Bitacora>();

            using (SqlConnection conexion = con.Conectar())
            {
                try
                {
                    // Abrimos la conexión aquí para ejecutar el lector de datos en modo forward-only.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand("sp_ObtenerBitacora", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // POR QUÉ: ejecutamos el lector y mapeamos manualmente cada fila para tener control total sobre conversiones y nulos.
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Bitacora b = new Bitacora();

                            // Mapeo explícito de columnas para evitar dependencia en el orden de columnas y controlar valores nulos.
                            b.IdBitacora = Convert.ToInt32(reader["IdBitacora"]);
                            b.IdUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            b.NombreUsuario = reader["NombreUsuario"].ToString();
                            b.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                            b.Modulo = reader["Modulo"].ToString();
                            b.Accion = reader["Accion"].ToString();

                            // Se comprueba DBNull para no provocar excepciones al convertir a string.
                            b.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null;
                            lista.Add(b);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: re-lanzamos con mensaje contextual para que la capa de negocio pueda mostrar un mensaje entendible por soporte o usuario.
                    throw new Exception("Error al obtener la bitácora: " + ex.Message, ex);
                }
            }

            return lista;
        }
    }
}
