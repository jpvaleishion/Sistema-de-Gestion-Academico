using System;
using System.Linq;
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

            // *cambio* - Evaluamos y ocultamos cada menú según la base de datos de permisos
            ConfigurarVisibilidad(mnuEstudiantes, "frmEstudiantes");
            ConfigurarVisibilidad(mnuDocentes, "frmDocentes");
            ConfigurarVisibilidad(mnuAsignaturas, "frmAsignaturas");
            ConfigurarVisibilidad(mnuCursos, "frmCursos");
            ConfigurarVisibilidad(mnuPeriodos, "frmPeriodos");
            ConfigurarVisibilidad(mnuMatriculas, "frmMatriculas");
            ConfigurarVisibilidad(mnuCalificaciones, "frmCalificaciones");
            ConfigurarVisibilidad(mnuUsuarios, "frmUsuarios");

            // *cambio* - Si los submenús están ocultos, ocultamos los menús contenedores principales
            // El menú de administración solo es visible si puedes ver la gestión de usuarios
            mnuAdministracion.Visible = mnuUsuarios.Visible;

            // Mantenimiento solo es visible si tienes permisos para ver estudiantes, docentes, asignaturas, cursos o periodos
            mnuMantenimiento.Visible = mnuEstudiantes.Visible ||
                                       mnuDocentes.Visible ||
                                       mnuAsignaturas.Visible ||
                                       mnuCursos.Visible ||
                                       mnuPeriodos.Visible;
        }
        // *cambio* - Método auxiliar para ocultar menús de forma dinámica
        private void ConfigurarVisibilidad(ToolStripMenuItem menu, string nombreFormulario)
        {
            // Busca si existe un permiso asignado en la sesión para este formulario específico
            var permiso = SesionActual.Permisos.FirstOrDefault(p => p.NombreFormulario.Equals(nombreFormulario, StringComparison.OrdinalIgnoreCase));

            // Si el permiso existe y tiene habilitado 'Visualizar', se muestra el menú. Si no, se oculta por completo.
            menu.Visible = permiso != null && permiso.Visualizar;
        }
    }
}

