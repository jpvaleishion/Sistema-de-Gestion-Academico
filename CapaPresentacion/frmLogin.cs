using CapaNegocio;
using System;
using System.Windows.Forms;
using static CapaPresentacion.Program;

namespace CapaPresentacion
{
    public partial class frmLogin : Form
    {
        private UsuarioServicio usuarioNegocio = new UsuarioServicio();
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
                // Intentar inicio de sesión seguro
                var usuario = usuarioNegocio.IniciarSesion(nombreUsuarioIntento, txtPassword.Text);

                // Guardar usuario en la sesión global
                SesionActual.UsuarioLogueado = usuario;

                // Cargamos dinámicamente los permisos del rol del usuario
                PermisoServicio permisoNegocio = new PermisoServicio();
                SesionActual.Permisos = permisoNegocio.ObtenerPermisosPorRol(usuario.IdRol);

                // Registro de éxito en la bitácora
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
            catch (InvalidOperationException ex)
            {
                // *cambio* - Captura errores controlados de negocio (Contraseña incorrecta, cuenta bloqueada, etc.)
                lblMensaje.Text = ex.Message; 
                RegistrarFalloBitacora(nombreUsuarioIntento, ex.Message);
                
                txtPassword.Clear();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                // *cambio* - Captura errores técnicos inesperados (Base de datos caída, error de red, etc.)
                // Mostramos un mensaje amigable al usuario y ocultamos el error técnico real por seguridad
                lblMensaje.Text = "Servicio no disponible temporalmente. Intente más tarde.";
                RegistrarFalloBitacora(nombreUsuarioIntento, $"Error crítico del sistema: {ex.Message}");
                
                txtPassword.Clear();
            }
        }
        // *cambio* - Método auxiliar para mantener limpio el flujo principal del botón ingresar
        private void RegistrarFalloBitacora(string usuarioIntento, string motivoFallo)
        {
            try
            {
                bitacoraNegocio.RegistrarAccion(
                    idUsuario: 0, // ID 0 porque no logró autenticarse
                    modulo: "Seguridad",
                    accion: "Login Fallido",
                    descripcion: $"Intento de acceso fallido con el usuario: '{usuarioIntento}'. Motivo: {motivoFallo}"
                );
            }
            catch
            {
                // Evitamos que un fallo al escribir en la bitácora (ej. si cayó la BD) rompa la UI
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}

