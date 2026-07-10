using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmUsuarios : Form
    {
        UsuarioServicio usuarioNegocio = new UsuarioServicio();
        int idSeleccionado = 0;

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
                Rol = cboRol.Text,
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
                usuarioNegocio.Guardar(ObtenerUsuarioDelFormulario());
                MessageBox.Show("Usuario guardado correctamente.");
                CargarGrid();
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Regla de Negocio / Error Operacional", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                usuarioNegocio.Actualizar(ObtenerUsuarioDelFormulario());
                MessageBox.Show("Usuario actualizado correctamente.");
                CargarGrid();
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Regla de Negocio / Error Operacional", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                usuarioNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Usuario eliminado correctamente.");
                CargarGrid();
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Regla de Negocio / Error Operacional", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmUsuarios_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void dgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var usuario = (Usuario)dgvUsuarios.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = usuario.IdUsuario;
            txtNombreUsuario.Text = usuario.NombreUsuario;
            txtPassword.Text = usuario.Password;
            cboRol.Text = usuario.Rol;
            cboEstado.Text = usuario.Estado;
        }
    }
}

