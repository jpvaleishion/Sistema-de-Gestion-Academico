using CapaNegocio;
using System;
using System.Windows.Forms;
using static CapaPresentacion.Program;

namespace CapaPresentacion
{
    public partial class frmLogin : Form
    {
        private UsuarioServicio usuarioNegocio = new UsuarioServicio();

        // *cambio* - Instanciamos el servicio de bitácora para registrar los accesos
        private BitacoraServicio bitacoraNegocio = new BitacoraServicio();
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

            string nombreUsuarioIntento = txtUsuario.Text.Trim();

            try
            {
                var usuario = usuarioNegocio.IniciarSesion(nombreUsuarioIntento, txtPassword.Text);

                SesionActual.UsuarioLogueado = usuario;

                // Cargamos dinámicamente los permisos del rol del usuario
                PermisoServicio permisoNegocio = new PermisoServicio();
                SesionActual.Permisos = permisoNegocio.ObtenerPermisosPorRol(usuario.IdRol);

                // *cambio* - Registro de éxito en la bitácora
                bitacoraNegocio.RegistrarAccion(
                    idUsuario: usuario.IdUsuario,
                    modulo: "Seguridad",
                    accion: "Login",
                    descripcion: $"Inicio de sesión exitoso para el usuario: {nombreUsuarioIntento}"
                );

                frmPrincipal principal = new frmPrincipal();

                // Al cerrarse el principal, mostramos de nuevo este Login
                principal.FormClosed += (s, args) => this.Show();

                principal.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = ex.Message;

                try
                {
                    // *cambio* - Registro de fallo en la bitácora
                    // Usamos ID 0 (o nulo) porque el usuario no logró autenticarse, pero dejamos constancia del intento
                    bitacoraNegocio.RegistrarAccion(
                        idUsuario: 0,
                        modulo: "Seguridad",
                        accion: "Login Fallido",
                        descripcion: $"Intento de acceso fallido con el usuario: '{nombreUsuarioIntento}'. Motivo: {ex.Message}"
                    );
                }
                catch
                {
                    // Evitamos que un fallo al escribir en la bitácora rompa el flujo visual del login
                }

                txtPassword.Clear();
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}

