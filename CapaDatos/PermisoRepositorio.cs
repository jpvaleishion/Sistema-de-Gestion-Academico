using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades.Entidades;

namespace CapaDatos
{
    /// <summary>
    /// Repositorio encargado de gestionar la consulta y verificación de permisos asignados a los roles del sistema.
    /// </summary>
    public class PermisoRepositorio
    {
        private Conexion conexion = new Conexion();

        /// <summary>
        /// Obtiene la lista completa de permisos asociados a un rol, utilizada para cargarlos en la sesión al iniciar sesión.
        /// </summary>
        /// <param name="idRol">Identificador del rol cuyos permisos se desean consultar.</param>
        /// <returns>Lista de objetos <see cref="Permiso"/> asociados al rol.</returns>
        public List<Permiso> ObtenerPermisosPorRol(int idRol)
        {
            List<Permiso> lista = new List<Permiso>();

            using (SqlConnection con = conexion.Conectar())
            {
                string query = @"SELECT P.IdPermiso, P.IdRol, P.IdMenu, M.NombreFormulario, 
                                        P.Visualizar, P.Crear, P.Modificar, P.Eliminar 
                                 FROM Permisos P
                                 INNER JOIN Menus M ON P.IdMenu = M.IdMenu
                                 WHERE P.IdRol = @IdRol";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdRol", idRol);
                    con.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Permiso
                            {
                                IdPermiso = Convert.ToInt32(rdr["IdPermiso"]),
                                IdRol = Convert.ToInt32(rdr["IdRol"]),
                                IdMenu = Convert.ToInt32(rdr["IdMenu"]),
                                NombreFormulario = rdr["NombreFormulario"].ToString(),
                                Visualizar = Convert.ToBoolean(rdr["Visualizar"]),
                                Crear = Convert.ToBoolean(rdr["Crear"]),
                                Modificar = Convert.ToBoolean(rdr["Modificar"]),
                                Eliminar = Convert.ToBoolean(rdr["Eliminar"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Verifica de forma rápida si un usuario tiene un permiso específico sobre un formulario determinado.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario a verificar.</param>
        /// <param name="nombreFormulario">Nombre del formulario sobre el cual se consulta el permiso.</param>
        /// <param name="operacion">Operación a verificar (visualizar, crear, modificar o eliminar).</param>
        /// <returns><c>true</c> si el usuario tiene el permiso solicitado; de lo contrario, <c>false</c>.</returns>
        public bool TienePermiso(int idUsuario, string nombreFormulario, string operacion)
        {
            using (SqlConnection con = conexion.Conectar())
            {
                string query = @"SELECT P.Visualizar, P.Crear, P.Modificar, P.Eliminar 
                                 FROM Usuarios U
                                 INNER JOIN Permisos P ON U.IdRol = P.IdRol
                                 INNER JOIN Menus M ON P.IdMenu = M.IdMenu
                                 WHERE U.IdUsuario = @IdUsuario AND M.NombreFormulario = @NombreFormulario";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@NombreFormulario", nombreFormulario);
                    con.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            bool visualizar = Convert.ToBoolean(rdr["Visualizar"]);
                            bool crear = Convert.ToBoolean(rdr["Crear"]);
                            bool modificar = Convert.ToBoolean(rdr["Modificar"]);
                            bool eliminar = Convert.ToBoolean(rdr["Eliminar"]);

                            switch (operacion.ToLower())
                            {
                                case "visualizar": return visualizar;
                                case "crear": return crear;
                                case "modificar": return modificar;
                                case "eliminar": return eliminar;
                                default: return false;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}