using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CapaDatos;
using CapaEntidades.Entidades;
using CapaNegocio.Logging;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de registros de bitácora.
    /// Permite registrar acciones de usuario y recuperar el historial completo.
    /// Implementa persistencia en repositorio y respaldo/lectura desde archivo.
    /// </summary>
    public class BitacoraServicio
    {
        private readonly BitacoraRepositorio repositorio = new BitacoraRepositorio();
        private readonly FileLogger _fileLogger;

        public BitacoraServicio(string rutaArchivo = null)
        {
            _fileLogger = new FileLogger(rutaArchivo);
        }

        /// <summary>
        /// Registra una acción en la bitácora del sistema aplicando validaciones.
        /// Intenta insertar en repositorio y siempre escribe una entrada en archivo como respaldo.
        /// Si la inserción en repositorio falla, se intenta escribir en archivo y se lanza InvalidOperationException.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario que ejecuta la acción.</param>
        /// <param name="modulo">Módulo del sistema donde ocurre la acción.</param>
        /// <param name="accion">Acción realizada (Crear, Modificar, Eliminar, etc.).</param>
        /// <param name="descripcion">Descripción detallada de la acción.</param>
        public void RegistrarAccion(int idUsuario, string modulo, string accion, string descripcion)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El identificador del usuario no es válido.", nameof(idUsuario));

            if (string.IsNullOrWhiteSpace(modulo))
                throw new ArgumentException("El módulo es obligatorio.", nameof(modulo));

            if (string.IsNullOrWhiteSpace(accion))
                throw new ArgumentException("La acción es obligatoria.", nameof(accion));

            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));

            // Normalizar valores
            string mod = modulo.Trim();
            string acc = accion.Trim();
            string desc = descripcion.Trim();
            DateTime fechaHora = DateTime.Now;

            // Construir entidad para BD
            Bitacora b = new Bitacora
            {
                IdUsuario = idUsuario,
                Modulo = mod,
                Accion = acc,
                Descripcion = desc,
                FechaHora = fechaHora
            };

            // Construir línea para archivo (una sola línea, sin saltos)
            string safeDesc = desc.Replace("\r", " ").Replace("\n", " ");
            string linea = $"{fechaHora.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)} | Usuario:{idUsuario} | Modulo:{Safe(mod)} | Accion:{Safe(acc)} | Descripcion:{safeDesc}";

            Exception repoException = null;

            // Intentar insertar en repositorio
            try
            {
                repositorio.Insertar(b);
            }
            catch (Exception ex)
            {
                repoException = ex;
            }

            // Intentar escribir en archivo (respaldo / duplicado)
            try
            {
                _fileLogger.LogLine(linea);
            }
            catch
            {
                // No propagar excepciones de archivo; si el repositorio falló, lo reportamos abajo.
            }

            // Si la inserción en repositorio falló, lanzar excepción controlada (manteniendo comportamiento previo)
            if (repoException != null)
            {
                // Intentamos dejar rastro en archivo (ya intentado arriba), ahora lanzamos la excepción original envuelta.
                throw new InvalidOperationException("Error al registrar la acción en la bitácora.", repoException);
            }
        }

        /// <summary>
        /// Obtiene todos los registros de bitácora almacenados.
        /// Primero intenta obtenerlos desde el repositorio; si falla, intenta leer y parsear el archivo de bitácora.
        /// </summary>
        /// <returns>Lista de registros de bitácora.</returns>
        public List<Bitacora> ObtenerTodo()
        {
            try
            {
                return repositorio.ObtenerTodo();
            }
            catch (Exception repoEx)
            {
                // Si falla el repositorio, intentamos recuperar desde el archivo de respaldo.
                try
                {
                    var lines = _fileLogger.ReadLastLines(int.MaxValue);
                    var list = new List<Bitacora>();

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        // Formato esperado:
                        // 2026-07-18 17:01:23.123 | Usuario:1 | Modulo:Asignaturas | Accion:Crear | Descripcion:Se registró...
                        var parts = line.Split('|').Select(p => p.Trim()).ToArray();
                        if (parts.Length < 5)
                        {
                            // línea no conforme, omitir
                            continue;
                        }

                        // FechaHora
                        DateTime fecha;
                        if (!DateTime.TryParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            // intentar parse más flexible
                            if (!DateTime.TryParse(parts[0], out fecha))
                                fecha = DateTime.MinValue;
                        }

                        // Usuario
                        int idUsuario = 0;
                        var usuarioPart = parts.FirstOrDefault(p => p.StartsWith("Usuario:", StringComparison.OrdinalIgnoreCase));
                        if (usuarioPart != null)
                        {
                            var val = usuarioPart.Substring("Usuario:".Length).Trim();
                            int.TryParse(val, out idUsuario);
                        }

                        // Modulo
                        string modulo = ExtractValue(parts, "Modulo:");

                        // Accion
                        string accion = ExtractValue(parts, "Accion:");

                        // Descripcion (todo lo que quede después de "Descripcion:")
                        string descripcion = string.Empty;
                        var descPart = parts.FirstOrDefault(p => p.StartsWith("Descripcion:", StringComparison.OrdinalIgnoreCase));
                        if (descPart != null)
                        {
                            descripcion = descPart.Substring("Descripcion:".Length).Trim();
                        }
                        else
                        {
                            // si no se encuentra, intentar tomar la última parte
                            descripcion = parts.Last();
                        }

                        var b = new Bitacora
                        {
                            IdUsuario = idUsuario,
                            Modulo = modulo,
                            Accion = accion,
                            Descripcion = descripcion,
                            FechaHora = fecha
                        };

                        list.Add(b);
                    }

                    return list;
                }
                catch (Exception fileEx)
                {
                    // Si ambos fallan, mantener el comportamiento previo y lanzar excepción con detalle.
                    throw new InvalidOperationException("Error al obtener los registros de bitácora.", new AggregateException(repoEx, fileEx));
                }
            }
        }

        /// <summary>
        /// Extrae el valor de una parte con prefijo (ej. "Modulo:Valor") dentro del arreglo de partes.
        /// </summary>
        private static string ExtractValue(string[] parts, string prefix)
        {
            var part = parts.FirstOrDefault(p => p.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (part == null) return string.Empty;
            return part.Substring(prefix.Length).Trim();
        }

        private string Safe(string s) => string.IsNullOrEmpty(s) ? "-" : s;
    }
}
