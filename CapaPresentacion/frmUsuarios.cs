using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmUsuarios : Form
    {
        private UsuarioServicio usuarioNegocio = new UsuarioServicio();
        private PermisoServicio permisoNegocio = new PermisoServicio();
        private RolServicio rolNegocio = new RolServicio();
        // Si cuentas con un RolServicio para llenar tu combobox, instáncialo aquí:
        // private RolServicio rolNegocio = new RolServicio();

        private int idUsuarioLogueado;
        private int idSeleccionado = 0;

        // *cambio* - Constructor estándar que recibe el ID de sesión del usuario activo
        public frmUsuarios(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto para compatibilidad con el Diseñador de Visual Studio
        public frmUsuarios()
        {
            InitializeComponent();
        }
        private void CargarGrid()
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = usuarioNegocio.ObtenerTodos();
        }


        private Usuario ObtenerUsuarioDelFormulario()
        {
            return new Usuario
            {
                IdUsuario = idSeleccionado,
                NombreUsuario = txtNombreUsuario.Text.Trim(),
                Password = txtPassword.Text,
                IdRol = cboRol.SelectedValue != null ? Convert.ToInt32(cboRol.SelectedValue) : 0, // *cambio* - Obtenemos el ID del rol
                Estado = cboEstado.Text
            };
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }


        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombreUsuario.Clear();
            txtPassword.Clear();
            cboRol.SelectedIndex = -1;
            cboEstado.SelectedIndex = -1;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                usuarioNegocio.Guardar(ObtenerUsuarioDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarGrid();
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Regla de Negocio / Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click_1(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un usuario de la lista.");
                return;
            }

            try
            {
                usuarioNegocio.Actualizar(ObtenerUsuarioDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarGrid();
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Regla de Negocio / Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo editar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un usuario de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este usuario?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                usuarioNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarGrid();
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Regla de Negocio / Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmUsuarios_Load_1(object sender, EventArgs e)
        {
            CargarComboRoles();       // Cargamos los Roles en el ComboBox de forma relacional
            CargarGrid();
            AplicarPermisosVisuales(); // Oculta o desactiva botones según permisos del rol
        }
        private void CargarComboRoles()
        {
            try
            {
                // Enlazamos la lista real de roles al ComboBox
                cboRol.DataSource = rolNegocio.ObtenerTodos();
                cboRol.DisplayMember = "NombreRol"; // Lo que el usuario ve
                cboRol.ValueMember = "IdRol";       // El valor interno que se guardará (ID)

                // Iniciamos con el combo limpio sin selección por defecto
                cboRol.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // *cambio* - Método unificado para restringir accesos usando PermisoServicio
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Evaluamos los accesos del rol activo para esta pantalla ("frmUsuarios")
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmUsuarios", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Bloqueo preventivo total en caso de fallo de validación
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }
        private void dgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var usuario = (Usuario)dgvUsuarios.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = usuario.IdUsuario;
            txtNombreUsuario.Text = usuario.NombreUsuario;
            txtPassword.Text = usuario.Password;
            cboRol.SelectedValue = usuario.IdRol; // *cambio* - Enlazamos el ID numérico al ComboBox
            cboEstado.Text = usuario.Estado;
        }
    }
}

