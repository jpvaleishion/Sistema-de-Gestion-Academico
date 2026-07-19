using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Asignatura"/>.
    /// Implementa las operaciones CRUD y mapeo simple entre filas de la base de datos y la entidad <see cref="Asignatura"/>.
    /// </summary>
    public class AsignaturaRepositorio
    {
        /// <summary>
        /// Componente encargado de proporcionar conexiones a la base de datos.
        /// Se mantiene como campo privado para reutilizar la fábrica de conexiones sin exponer detalles de infraestructura.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta una nueva asignatura en la base de datos.
        /// </summary>
        /// <param name="a">Objeto <see cref="Asignatura"/> con los datos a insertar. No debe ser null.</param>
        /// <returns>Void. La operación persiste datos en la base; si ocurre un error se lanza una excepción.</returns>
        /// <exception cref="System.ArgumentNullException">Se lanzará si <paramref name="a"/> es null (no comprobado localmente; el llamador debe validar).</exception>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) y las propaga con mensaje contextual.</exception>
        public void Insertar(Asignatura a)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Asignaturas (Nombre,Creditos,Modalidad,Estado) VALUES (@Nombre, @Creditos, @Modalidad, @Estado)";
                try
                {
                    // Abrimos explícitamente la conexión aquí para controlar el alcance y liberar recursos inmediatamente
                    // De esta forma, el using garantiza que la conexión se cierre correctamente aunque se lance una excepción.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Uso de parámetros para prevenir inyección SQL y para permitir el manejo correcto de tipos.
                        command.Parameters.AddWithValue("@Nombre", a.Nombre);
                        command.Parameters.AddWithValue("@Creditos", a.Creditos);
                        command.Parameters.AddWithValue("@Modalidad", a.Modalidad);
                        command.Parameters.AddWithValue("@Estado", a.Estado);

                        // Ejecutamos la operación de inserción. No necesitamos el número de filas afectadas en este contexto.
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // Se envuelve SqlException en Exception general para mantener una interfaz homogénea hacia capas superiores.
                    // POR QUÉ: mantener la excepción original como InnerException permite diagnóstico posterior sin filtrar detalles de infraestructura aquí.
                    throw new Exception("Error al insertar la asignatura: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de una asignatura existente.
        /// </summary>
        /// <param name="a">Objeto <see cref="Asignatura"/> con los datos actualizados. Debe contener <see cref="Asignatura.IdAsignatura"/> válido.</param>
        /// <returns>Void. Realiza la actualización en la base de datos. Si no existe la fila correspondiente, la consulta no modifica filas.</returns>
        /// <exception cref="System.Exception">Envuelve y propaga excepciones de SQL con contexto adicional.</exception>
        public void Actualizar(Asignatura a)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Asignaturas SET Nombre=@Nombre, Creditos=@Creditos, Modalidad=@Modalidad, Estado=@Estado WHERE IdAsignatura=@IdAsignatura";
                try
                {
                    // Abrimos la conexión justo antes de ejecutar el comando para reducir la ventana en la que la conexión permanece abierta.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Parámetros explícitos para evitar errores de conversión y proteger contra inyección SQL.
                        command.Parameters.AddWithValue("@IdAsignatura", a.IdAsignatura);
                        command.Parameters.AddWithValue("@Nombre", a.Nombre);
                        command.Parameters.AddWithValue("@Creditos", a.Creditos);
                        command.Parameters.AddWithValue("@Modalidad", a.Modalidad);
                        command.Parameters.AddWithValue("@Estado", a.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: re-lanzamos con mensaje contextual para que la capa de negocio tenga información legible sin exponer detalles de SQL Server en cada lugar.
                    throw new Exception("Error al actualizar la asignatura: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina una asignatura de la base de datos según su identificador.
        /// </summary>
        /// <param name="idAsignatura">Identificador de la asignatura a eliminar.</param>
        /// <returns>Void. Si no existe la fila, la operación no tiene efecto.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL y las propaga con contexto.</exception>
        public void Eliminar(int idAsignatura)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Asignaturas WHERE IdAsignatura=@IdAsignatura";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Eliminamos por clave primaria; usar parámetro evita problemas con formatos y ataques de inyección.
                        command.Parameters.AddWithValue("@IdAsignatura", idAsignatura);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar la asignatura: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de asignaturas registradas.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Asignatura"/>. Devuelve una lista vacía si no hay registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve y propaga excepciones de SQL.</exception>
        public List<Asignatura> ObtenerTodos()
        {
            List<Asignatura> lista = new List<Asignatura>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Asignaturas";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Ejecutamos el lector de datos en modo forward-only para eficiencia.
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Asignatura a = new Asignatura();
                            // Mapear manualmente los campos para evitar dependencia en orden de columnas y controlar conversiones.
                            // POR QUÉ: Convert.ToInt32 y ToString se usan explícitamente para controlar valores nulos o formatos inesperados en la BD.
                            a.IdAsignatura = Convert.ToInt32(reader["IdAsignatura"]);
                            a.Nombre = reader["Nombre"].ToString();
                            a.Creditos = Convert.ToInt32(reader["Creditos"]);
                            a.Modalidad = reader["Modalidad"].ToString();
                            a.Estado = reader["Estado"].ToString();
                            lista.Add(a);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: Se envuelve la excepción para dar contexto de negocio y permitir a la capa superior decidir cómo manejarla.
                    throw new Exception("Error al obtener las asignaturas: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene una asignatura específica según su identificador.
        /// </summary>
        /// <param name="idAsignatura">Identificador de la asignatura a buscar.</param>
        /// <returns>Objeto <see cref="Asignatura"/> encontrado, o <c>null</c> si no existe la asignatura.</returns>
        /// <exception cref="System.Exception">Envuelve y propaga excepciones de SQL.</exception>
        public Asignatura ObtenerPorId(int idAsignatura)
        {
            Asignatura a = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Asignaturas WHERE IdAsignatura=@IdAsignatura";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Búsqueda por PK mediante parámetro para evitar inyección y problemas de conversión.
                        command.Parameters.AddWithValue("@IdAsignatura", idAsignatura);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            a = new Asignatura();
                            // Mapeo explícito de columnas para garantizar consistencia aun si se reordenan columnas en la consulta.
                            a.IdAsignatura = Convert.ToInt32(reader["IdAsignatura"]);
                            a.Nombre = reader["Nombre"].ToString();
                            a.Creditos = Convert.ToInt32(reader["Creditos"]);
                            a.Modalidad = reader["Modalidad"].ToString();
                            a.Estado = reader["Estado"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la asignatura: " + ex.Message, ex);
                }
            }

            return a;
        }
    }
}
