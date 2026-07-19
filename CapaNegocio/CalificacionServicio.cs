using CapaDatos;
using CapaEntidades.Entidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    /// <summary>
    /// Servicio de negocio para la gestión de calificaciones académicas.
    /// Aplica reglas de negocio sobre notas, cálculo de nota final, estado y auditoría.
    /// Se integra registro de errores en la bitácora sin cambiar firmas públicas.
    /// </summary>
    public class CalificacionServicio
    {
        private readonly CalificacionRepositorio repositorio = new CalificacionRepositorio();
        private readonly PermisoServicio permisoService = new PermisoServicio();
        private readonly BitacoraServicio bitacoraService = new BitacoraServicio();

        /// <summary>
        /// Registra un error en la bitácora. No modifica la estructura de la bitácora existente.
        /// </summary>
        /// <param name="ex">Excepción capturada.</param>
        /// <param name="idUsuario">Identificador del usuario relacionado con la acción (0 si no aplica).</param>
        /// <param name="modulo">Nombre del módulo donde ocurrió el error.</param>
        /// <param name="accion">Acción que se estaba ejecutando cuando ocurrió el error.</param>
        /// <param name="contexto">Contexto adicional para diagnóstico.</param>
        private void RegistrarErrorEnBitacora(Exception ex, int idUsuario, string modulo, string accion, string contexto)
        {
            try
            {
                string descripcion = $"Contexto: {contexto} | Detalle: {ex.ToString()}";
                bitacoraService.RegistrarAccion(idUsuario, modulo, "Error", descripcion);
            }
            catch
            {
                // No propagar excepciones desde el registro de errores para no afectar la lógica de negocio.
            }
        }

        /// <summary>
        /// Registra una nueva calificación aplicando validaciones, cálculo de nota final y auditoría.
        /// </summary>
        /// <param name="c">Objeto <see cref="Calificacion"/> con datos a guardar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la operación.</param>
        /// <exception cref="InvalidOperationException">Si el usuario no tiene permiso o ocurre un error interno.</exception>
        public void Guardar(Calificacion c, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Crear"))
                    throw new InvalidOperationException("No tiene permisos para registrar calificaciones.");

                Validar(c);

                c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
                c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);
                c.FechaCalificacion = DateTime.Now;

                repositorio.Insertar(c);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Crear",
                    $"Se registró calificación para Matrícula ID {c.IdMatricula}. Nota1: {c.Nota1}, Nota2: {c.Nota2}, NotaFinal: {c.NotaFinal} ({c.Estado})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Crear",
                    $"Intentando guardar calificación MatrículaID={(c != null ? c.IdMatricula.ToString() : "null")}, Nota1={(c != null ? c.Nota1.ToString() : "null")}, Nota2={(c != null ? c.Nota2.ToString() : "null")}"
                );

                // POR QUÉ: Mantener comportamiento previo: envolver en InvalidOperationException si no lo es ya
                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al guardar la calificación.", ex);
            }
        }

        /// <summary>
        /// Actualiza una calificación existente aplicando validaciones, recálculo y auditoría.
        /// </summary>
        /// <param name="c">Objeto <see cref="Calificacion"/> con los datos actualizados.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la operación.</param>
        public void Actualizar(Calificacion c, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Modificar"))
                    throw new InvalidOperationException("No tiene permisos para modificar calificaciones.");

                Validar(c);

                c.NotaFinal = CalcularNotaFinal(c.Nota1, c.Nota2);
                c.Estado = CalcularEstado(c.NotaFinal, c.NotaMaxima);

                repositorio.Actualizar(c);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Modificar",
                    $"Se actualizó la Calificación ID {c.IdCalificacion} (Matrícula ID: {c.IdMatricula}). Nueva NotaFinal: {c.NotaFinal} ({c.Estado})."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Modificar",
                    $"Intentando actualizar calificación ID={(c != null ? c.IdCalificacion.ToString() : "null")}"
                );

                // POR QUÉ: Preservamos el comportamiento existente dejando que InvalidOperationException y ArgumentException se propaguen.
                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al actualizar la calificación.", ex);
            }
        }

        /// <summary>
        /// Elimina una calificación y registra la acción en la bitácora.
        /// </summary>
        /// <param name="idCalificacion">Identificador de la calificación a eliminar.</param>
        /// <param name="idUsuarioLogueado">Identificador del usuario que realiza la operación.</param>
        public void Eliminar(int idCalificacion, int idUsuarioLogueado)
        {
            try
            {
                if (!permisoService.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Eliminar"))
                    throw new InvalidOperationException("No tiene permisos para eliminar calificaciones.");

                if (idCalificacion <= 0)
                    throw new ArgumentException("El identificador de la calificación no es válido.");

                string detalleCalificacion = $"ID {idCalificacion}";
                try
                {
                    var califPrev = repositorio.ObtenerPorId(idCalificacion);
                    if (califPrev != null)
                        detalleCalificacion = $"ID {idCalificacion} (Matrícula ID: {califPrev.IdMatricula}, NotaFinal previa: {califPrev.NotaFinal})";
                }
                catch (Exception exObtener)
                {
                    RegistrarErrorEnBitacora(exObtener, idUsuarioLogueado, "Calificaciones", "Eliminar", $"Obteniendo detalle para ID={idCalificacion}");
                }

                repositorio.Eliminar(idCalificacion);

                bitacoraService.RegistrarAccion(
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Eliminar",
                    $"Se eliminó la calificación: {detalleCalificacion}."
                );
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(
                    ex,
                    idUsuarioLogueado,
                    "Calificaciones",
                    "Eliminar",
                    $"Intentando eliminar calificación ID={idCalificacion}"
                );

                // POR QUÉ: Preservamos InvalidOperationException y ArgumentException; otros errores se envuelven para contexto.
                if (ex is InvalidOperationException || ex is ArgumentException)
                    throw;
                throw new InvalidOperationException("Error al eliminar la calificación.", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las calificaciones registradas.
        /// </summary>
        /// <returns>Lista de calificaciones.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error al recuperar las calificaciones.</exception>
        public List<Calificacion> ObtenerTodos()
        {
            try
            {
                return repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Calificaciones", "ObtenerTodos", "Obteniendo todas las calificaciones");
                throw new InvalidOperationException("Error al obtener las calificaciones.", ex);
            }
        }

        /// <summary>
        /// Obtiene una calificación por su identificador.
        /// </summary>
        /// <param name="idCalificacion">Identificador de la calificación.</param>
        /// <returns>Instancia de <see cref="Calificacion"/> o null si no existe.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error al recuperar la calificación.</exception>
        public Calificacion ObtenerPorId(int idCalificacion)
        {
            try
            {
                if (idCalificacion <= 0)
                    throw new ArgumentException("El identificador de la calificación no es válido.");

                return repositorio.ObtenerPorId(idCalificacion);
            }
            catch (Exception ex)
            {
                RegistrarErrorEnBitacora(ex, 0, "Calificaciones", "ObtenerPorId", $"Obteniendo calificación ID={idCalificacion}");
                throw new InvalidOperationException("Error al obtener la calificación.", ex);
            }
        }

        // VALIDACIONES (RN-04)
        /// <summary>
        /// Valida la entidad de calificación según reglas de negocio.
        /// </summary>
        /// <param name="c">Objeto <see cref="Calificacion"/> a validar.</param>
        /// <exception cref="ArgumentNullException">Si la calificación es nula.</exception>
        private void Validar(Calificacion c)
        {
            if (c == null)
                throw new ArgumentNullException(nameof(c), "La calificación no puede ser nula.");

            if (c.IdMatricula <= 0)
                throw new ArgumentException("Debe seleccionar una matrícula.");

            if (c.NotaMaxima <= 0)
                throw new ArgumentException("La nota máxima debe ser mayor a 0.");

            if (c.Nota1 < 0 || c.Nota1 > c.NotaMaxima)
                throw new ArgumentException("Nota1 debe estar entre 0 y la nota máxima.");

            if (c.Nota2 < 0 || c.Nota2 > c.NotaMaxima)
                throw new ArgumentException("Nota2 debe estar entre 0 y la nota máxima.");

            if (!string.IsNullOrWhiteSpace(c.Observaciones) && c.Observaciones.Length > 250)
                throw new ArgumentException("Las observaciones no pueden superar los 250 caracteres.");
        }

        // CALCULO NOTA FINAL (RN-06)
        /// <summary>
        /// Calcula la nota final promediando Nota1 y Nota2 con redondeo a dos decimales.
        /// </summary>
        /// <param name="nota1">Primera nota.</param>
        /// <param name="nota2">Segunda nota.</param>
        /// <returns>Nota final redondeada a 2 decimales.</returns>
        private decimal CalcularNotaFinal(decimal nota1, decimal nota2)
        {
            return Math.Round((nota1 + nota2) / 2m, 2);
        }

        // CALCULO ESTADO ACADEMICO (RN-07)
        /// <summary>
        /// Calcula el estado académico (Aprobado/Supletorio/Reprobado) según porcentaje sobre la nota máxima.
        /// </summary>
        /// <param name="notaFinal">Nota final calculada.</param>
        /// <param name="notaMaxima">Valor de la nota máxima para la asignatura.</param>
        /// <returns>Cadena con el estado académico.</returns>
        private string CalcularEstado(decimal notaFinal, decimal notaMaxima)
        {
            if (notaMaxima <= 0)
                throw new InvalidOperationException("La nota máxima debe ser mayor a 0 para calcular el estado.");

            decimal porcentaje = (notaFinal / notaMaxima) * 100m;

            if (porcentaje >= 70m)
                return "Aprobado";
            else if (porcentaje >= 50m)
                return "Supletorio";
            else
                return "Reprobado";
        }
    }
}
