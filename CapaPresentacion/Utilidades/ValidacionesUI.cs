using CapaEntidades.Entidades;
using System;
using System.Windows.Forms;

namespace CapaPresentacion.Utilidades
{
    /// <summary>
    /// Proporciona utilidades reutilizables para validar la entrada de texto en controles WinForms.
    /// </summary>
    public static class ValidacionesUI
    {
        /// <summary>
        /// Permite únicamente letras Unicode y espacios en un control de texto, evitando espacios iniciales o dobles espacios consecutivos.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento de pulsación de tecla.</param>
        /// <param name="e">Argumentos del evento de tecla que contienen el carácter ingresado.</param>
        public static void PermitirSoloLetrasYEspacio(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            var textBox = sender as TextBox;

            if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            if (e.KeyChar == ' ')
            {
                if (textBox == null)
                {
                    e.Handled = true;
                    return;
                }

                if (textBox.SelectionStart == 0)
                {
                    e.Handled = true;
                    return;
                }

                if (textBox.SelectionStart > 0 && textBox.Text.Length > 0)
                {
                    var anterior = textBox.Text[textBox.SelectionStart - 1];
                    if (anterior == ' ')
                    {
                        e.Handled = true;
                        return;
                    }
                }

                e.Handled = false;
                return;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Permite únicamente dígitos en un control de texto para el ingreso de números telefónicos, respetando una longitud máxima configurada.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento de pulsación de tecla.</param>
        /// <param name="e">Argumentos del evento de tecla que contienen el carácter ingresado.</param>
        /// <param name="maxLongitud">La longitud máxima permitida para el texto ingresado. El valor predeterminado es 10.</param>
        public static void PermitirSoloNumerosTelefono(object sender, KeyPressEventArgs e, int maxLongitud = 10)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            var textBox = sender as TextBox;
            if (textBox == null)
            {
                e.Handled = true;
                return;
            }

            int longitudActual = textBox.Text.Length - textBox.SelectionLength;
            if (char.IsDigit(e.KeyChar) && longitudActual < maxLongitud)
            {
                e.Handled = false;
                return;
            }

            e.Handled = true;
        }

        public static bool ValidarFechasMatriculaPeriodo(PeriodoAcademico periodo, out string mensaje)
        {
            mensaje = string.Empty;
            if (periodo == null) return true;

            DateTime hoy = DateTime.Now.Date;
            DateTime fechaApertura = periodo.FechaInicio.AddDays(-15).Date;
            DateTime fechaLimite = periodo.FechaInicio.AddDays(-5).Date;

            if (hoy < fechaApertura)
            {
                mensaje = $"Atención: Aún no está abierto el período de matrícula para este ciclo.\n\nAbre oficialmente el: {fechaApertura:dd/MM/yyyy}.";
                return false;
            }

            if (hoy > fechaLimite)
            {
                mensaje = $"Atención: El plazo de matrícula para este período ya finalizó.\n\nLa fecha límite fue el: {fechaLimite:dd/MM/yyyy} (5 días antes del inicio del período).";
                return false;
            }

            return true;
        }
    }
}
