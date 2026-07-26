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
            if (char.IsControl(e.KeyChar)) // Permite teclas de control como Backspace, Delete, etc.
            {
                e.Handled = false; // No bloquea la entrada de teclas de control
                return;
            }

            var textBox = sender as TextBox;

            if (char.IsLetter(e.KeyChar)) // Permite letras Unicode
            {
                e.Handled = false; 
                return;
            }

            if (e.KeyChar == ' ') // Permite espacios, pero con restricciones
            {
                if (textBox == null) // Si no es un TextBox, no permitimos el espacio
                {
                    e.Handled = true;
                    return;
                }

                if (textBox.SelectionStart == 0) // Evita espacios al inicio del texto
                {
                    e.Handled = true;
                    return;
                }

                if (textBox.SelectionStart > 0 && textBox.Text.Length > 0) // Evita espacios dobles consecutivos
                {
                    var anterior = textBox.Text[textBox.SelectionStart - 1]; // Obtiene el carácter anterior al cursor
                    if (anterior == ' ') // Si el carácter anterior es un espacio, no permitimos otro espacio
                    {
                        e.Handled = true; // Bloquea la entrada del espacio
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

            var textBox = sender as TextBox; // Asegura que el sender sea un TextBox
            if (textBox == null)
            {
                e.Handled = true;
                return;
            }

            int longitudActual = textBox.Text.Length - textBox.SelectionLength; // Calcula la longitud actual del texto considerando la selección
            if (char.IsDigit(e.KeyChar) && longitudActual < maxLongitud) // Permite dígitos si no se excede la longitud máxima
            {
                e.Handled = false;
                return;
            }

            e.Handled = true;
        }

        public static bool ValidarFechasMatriculaPeriodo(PeriodoAcademico periodo, out string mensaje)
        {
            mensaje = string.Empty;
            if (periodo == null) return true; // Si el período es nulo, no se realiza ninguna validación y se considera válido.

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
