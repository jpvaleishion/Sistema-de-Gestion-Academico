using CapaEntidades.Entidades;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
        }
        // Guarda el usuario logueado para usarlo en toda la aplicación
        public static class SesionActual
        {
            public static Usuario UsuarioLogueado { get; set; }
        }

    }
}
