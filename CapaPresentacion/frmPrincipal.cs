using System;
using System.Windows.Forms;
using static CapaPresentacion.Program;

namespace CapaPresentacion
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
        }

        // Cada opción del menú abre su formulario hijo dentro del MDI


        private void AbrirHija(Form formularioHijo)
        {
            foreach (Form formularioAbierto in this.MdiChildren)
            {
                if (formularioAbierto.GetType() == formularioHijo.GetType())
                {
                    formularioAbierto.Activate();
                    formularioAbierto.Focus();
                    formularioHijo.Dispose();
                    return;
                }
            }

            formularioHijo.MdiParent = this;

            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.ControlBox = false;
            formularioHijo.Text = "";

            formularioHijo.WindowState = FormWindowState.Maximized;
            formularioHijo.Show();
        }

        private void mnuEstudiantes_Click(object sender, EventArgs e) => AbrirHija(new frmEstudiantes());
        private void mnuDocentes_Click(object sender, EventArgs e) => AbrirHija(new frmDocentes());
        private void mnuAsignaturas_Click(object sender, EventArgs e) => AbrirHija(new frmAsignaturas());
        private void mnuCursos_Click(object sender, EventArgs e) => AbrirHija(new frmCursos());
        private void mnuPeriodos_Click(object sender, EventArgs e) => AbrirHija(new frmPeriodos());
        private void mnuMatriculas_Click(object sender, EventArgs e) => AbrirHija(new frmMatriculas());
        private void mnuCalificaciones_Click(object sender, EventArgs e) => AbrirHija(new frmCalificaciones());
        private void mnuUsuarios_Click(object sender, EventArgs e) => AbrirHija(new frmUsuarios());

        private void mnuSalir_Click(object sender, EventArgs e) => Application.Exit();

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            var usuario = SesionActual.UsuarioLogueado;

            // Muestra el usuario y rol en la barra de estado
            lblUsuarioActivo.Text = "Usuario: " + usuario.NombreUsuario + "   |   Rol: " + usuario.Rol;

            // Solo el Administrador puede gestionar usuarios
            mnuAdministracion.Visible = usuario.Rol == "Administrador";

            // Solo Administrador y Secretaria gestionan mantenimiento y matrículas
            bool esAdminOSecretaria = usuario.Rol == "Administrador" || usuario.Rol == "Secretaria";
            mnuMantenimiento.Visible = esAdminOSecretaria;
            mnuMatriculas.Visible = esAdminOSecretaria;

            // Calificaciones la usan Administrador y Docente
            mnuCalificaciones.Visible = usuario.Rol == "Administrador" || usuario.Rol == "Docente";
        }
    }
}

