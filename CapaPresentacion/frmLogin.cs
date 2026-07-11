using CapaNegocio;
using System;
using System.Windows.Forms;
using static CapaPresentacion.Program;

namespace CapaPresentacion
{
    public partial class frmLogin : Form
    {
        UsuarioServicio usuarioNegocio = new UsuarioServicio();

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click_1(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMensaje.Text = "Ingrese usuario y contraseña.";
                return;
            }

            try
            {
                var usuario = usuarioNegocio.IniciarSesion(txtUsuario.Text.Trim(), txtPassword.Text);

                SesionActual.UsuarioLogueado = usuario;

                frmPrincipal principal = new frmPrincipal();
                principal.Show();
                this.Hide();
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = ex.Message;
                txtPassword.Clear(); 
            }
            catch (InvalidOperationException ex)
            {
                lblMensaje.Text = ex.Message;
                txtPassword.Clear(); 
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error inesperado: " + ex.Message;
                txtPassword.Clear(); 
            }
        }
    }
}

