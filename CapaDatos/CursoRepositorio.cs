using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones de acceso a datos para la entidad <see cref="Curso"/>.
    /// Implementa operaciones CRUD básicas y mapeo manual de filas a la entidad.
    /// </summary>
    public class CursoRepositorio
    {
        /// <summary>
        /// Componente encargado de proporcionar conexiones a la base de datos.
        /// Se mantiene privado para controlar la creación de conexiones desde la capa de datos.
        /// </summary>
        private Conexion con = new Conexion();

        /// <summary>
        /// Inserta un nuevo curso en la base de datos.
        /// </summary>
        /// <param name="c">Objeto <see cref="Curso"/> con los datos a insertar. No debe ser null; el llamador es responsable de validar.</param>
        /// <returns>Void. Persiste el registro en la tabla Cursos.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL (SqlException) y las propaga con contexto adicional.</exception>
        public void Insertar(Curso c)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "INSERT INTO Cursos (NombreCurso,Paralelo,Capacidad,Estado) VALUES (@NombreCurso, @Paralelo, @Capacidad, @Estado)";
                try
                {
                    // Abrimos la conexión lo más tarde posible para minimizar la ventana en la que la conexión permanece abierta.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Uso de parámetros para prevenir inyección SQL y asegurar el correcto tratamiento de tipos por el proveedor.
                        command.Parameters.AddWithValue("@NombreCurso", c.NombreCurso);
                        command.Parameters.AddWithValue("@Paralelo", c.Paralelo);
                        command.Parameters.AddWithValue("@Capacidad", c.Capacidad);
                        command.Parameters.AddWithValue("@Estado", c.Estado);

                        // Ejecutamos la inserción; no se requiere retorno de identidad en este método.
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la excepción para añadir contexto desde la capa de datos y mantener trazabilidad.
                    throw new Exception("Error al insertar el curso: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un curso existente.
        /// </summary>
        /// <param name="c">Objeto <see cref="Curso"/> con los datos actualizados. Debe contener IdCurso válido.</param>
        /// <returns>Void. Si la fila no existe, la consulta no modificará registros.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Actualizar(Curso c)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "UPDATE Cursos SET NombreCurso=@NombreCurso, Paralelo=@Paralelo, Capacidad=@Capacidad, Estado=@Estado WHERE IdCurso=@IdCurso";
                try
                {
                    // Abrimos la conexión inmediatamente antes de ejecutar el comando para acotar el tiempo de uso del recurso.
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Parámetros explícitos para evitar problemas de conversión y ataques de inyección.
                        command.Parameters.AddWithValue("@IdCurso", c.IdCurso);
                        command.Parameters.AddWithValue("@NombreCurso", c.NombreCurso);
                        command.Parameters.AddWithValue("@Paralelo", c.Paralelo);
                        command.Parameters.AddWithValue("@Capacidad", c.Capacidad);
                        command.Parameters.AddWithValue("@Estado", c.Estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: re-lanzamos con mensaje contextual para que la capa superior pueda registrar o manejar el error adecuadamente.
                    throw new Exception("Error al actualizar el curso: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Elimina un curso de la base de datos según su identificador.
        /// </summary>
        /// <param name="idCurso">Identificador del curso a eliminar.</param>
        /// <returns>Void. Operación realiza un DELETE físico sobre la tabla Cursos.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public void Eliminar(int idCurso)
        {
            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "DELETE FROM Cursos WHERE IdCurso=@IdCurso";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Eliminación por clave primaria utilizando parámetro para evitar inyección.
                        command.Parameters.AddWithValue("@IdCurso", idCurso);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar el curso: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista completa de cursos registrados.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Curso"/>. Devuelve una lista vacía si no hay registros; nunca devuelve null.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public List<Curso> ObtenerTodos()
        {
            List<Curso> lista = new List<Curso>();

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Cursos";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        // Ejecutamos lector en modo forward-only implícito para eficiencia.
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Curso c = new Curso();
                            // Mapeo explícito para controlar conversiones y evitar dependencia del orden de columnas.
                            c.IdCurso = Convert.ToInt32(reader["IdCurso"]);
                            c.NombreCurso = reader["NombreCurso"].ToString();
                            c.Paralelo = reader["Paralelo"].ToString();
                            c.Capacidad = Convert.ToInt32(reader["Capacidad"]);
                            c.Estado = reader["Estado"].ToString();
                            lista.Add(c);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // POR QUÉ: encapsulamos la excepción con contexto para facilitar diagnósticos desde la capa de negocio.
                    throw new Exception("Error al obtener los cursos: " + ex.Message, ex);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un curso específico según su identificador.
        /// </summary>
        /// <param name="idCurso">Identificador del curso a buscar.</param>
        /// <returns>Objeto <see cref="Curso"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="System.Exception">Envuelve excepciones de SQL.</exception>
        public Curso ObtenerPorId(int idCurso)
        {
            Curso c = null;

            using (SqlConnection conexion = con.Conectar())
            {
                string sql = "SELECT * FROM Cursos WHERE IdCurso=@IdCurso";
                try
                {
                    conexion.Open();
                    using (SqlCommand command = new SqlCommand(sql, conexion))
                    {
                        command.Parameters.AddWithValue("@IdCurso", idCurso);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            c = new Curso();
                            // Mapeo manual para asegurar tipos y nullables según contrato de la entidad.
                            c.IdCurso = Convert.ToInt32(reader["IdCurso"]);
                            c.NombreCurso = reader["NombreCurso"].ToString();
                            c.Paralelo = reader["Paralelo"].ToString();
                            c.Capacidad = Convert.ToInt32(reader["Capacidad"]);
                            c.Estado = reader["Estado"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el curso: " + ex.Message, ex);
                }
            }

            return c;
        }
    }
}
