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

        // *cambios* - Ahora pasamos el ID del usuario logueado en el constructor de cada formulario hijo
        private void mnuEstudiantes_Click(object sender, EventArgs e) =>
            AbrirHija(new frmEstudiantes(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuDocentes_Click(object sender, EventArgs e) =>
            AbrirHija(new frmDocentes(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuAsignaturas_Click(object sender, EventArgs e) =>
            AbrirHija(new frmAsignaturas(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuCursos_Click(object sender, EventArgs e) =>
            AbrirHija(new frmCursos(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuPeriodos_Click(object sender, EventArgs e) =>
            AbrirHija(new frmPeriodos(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuMatriculas_Click(object sender, EventArgs e) =>
            AbrirHija(new frmMatriculas(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuCalificaciones_Click(object sender, EventArgs e) =>
            AbrirHija(new frmCalificaciones(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuUsuarios_Click(object sender, EventArgs e) =>
            AbrirHija(new frmUsuarios(SesionActual.UsuarioLogueado.IdUsuario));

        private void mnuSalir_Click(object sender, EventArgs e) => Application.Exit();
        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            var usuario = SesionActual.UsuarioLogueado;

            // Muestra el usuario y rol en la barra de estado
            lblUsuarioActivo.Text = "Usuario: " + usuario.NombreUsuario + "   |   Rol: " + usuario.NombreRol;

            // Evaluamos y ocultamos cada menú según la base de datos de permisos
            ConfigurarVisibilidad(mnuEstudiantes, "frmEstudiantes");
            ConfigurarVisibilidad(mnuDocentes, "frmDocentes");
            ConfigurarVisibilidad(mnuAsignaturas, "frmAsignaturas");
            ConfigurarVisibilidad(mnuCursos, "frmCursos");
            ConfigurarVisibilidad(mnuPeriodos, "frmPeriodos");
            ConfigurarVisibilidad(mnuMatriculas, "frmMatriculas");
            ConfigurarVisibilidad(mnuCalificaciones, "frmCalificaciones");
            ConfigurarVisibilidad(mnuUsuarios, "frmUsuarios");

            // SOLUCIÓN AL BUG: Usamos .Available en lugar de .Visible para leer el estado real de los hijos

            // El menú de administración solo es visible si puedes ver la gestión de usuarios
            mnuAdministracion.Visible = mnuUsuarios.Available;

            // Mantenimiento solo es visible si tienes permisos para ver estudiantes, docentes, asignaturas, cursos o periodos
            mnuMantenimiento.Visible = mnuEstudiantes.Available ||
                                       mnuDocentes.Available ||
                                       mnuAsignaturas.Available ||
                                       mnuCursos.Available ||
                                       mnuPeriodos.Available;

            // Procesos solo es visible si tienes permisos para Matrículas o Calificaciones
            // (Esto corrige el detalle de que Procesos se quede visible siempre por defecto)
            mnuProcesos.Visible = mnuMatriculas.Available ||
                                  mnuCalificaciones.Available;
        }
        // *cambio* - Método auxiliar para ocultar menús de forma dinámica
        private void ConfigurarVisibilidad(ToolStripMenuItem menu, string nombreFormulario)
        {
            if (SesionActual.Permisos == null)
            {
                menu.Visible = false;
                return;
            }

            // Buscamos ignorando mayúsculas/minúsculas y eliminando espacios en blanco a los lados (.Trim())
            var permiso = SesionActual.Permisos.FirstOrDefault(p =>
                p.NombreFormulario != null &&
                p.NombreFormulario.Trim().Equals(nombreFormulario.Trim(), StringComparison.OrdinalIgnoreCase));

            // Si el permiso existe y tiene Visualizar en true, se muestra
            menu.Visible = permiso != null && permiso.Visualizar;
        }
    }
}

