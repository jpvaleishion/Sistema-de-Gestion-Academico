using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmDocentes : Form
    {
        DocenteServicio docenteNegocio = new DocenteServicio();
        int idSeleccionado = 0;

        public frmDocentes()
        {
            InitializeComponent();
        }



        private void CargarGrid()
        {
            dgvDocentes.DataSource = null;
            dgvDocentes.DataSource = docenteNegocio.ObtenerTodos();
        }


        private Docente ObtenerDocenteDelFormulario()
        {
            return new Docente
            {
                IdPersona = idSeleccionado,
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Especialidad = txtEspecialidad.Text.Trim(),
                Estado = cboEstado.Text
            };
        }



        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombres.Clear();
            txtApellidos.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtEspecialidad.Clear();
            cboEstado.SelectedIndex = -1;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                docenteNegocio.Guardar(ObtenerDocenteDelFormulario());
                MessageBox.Show("Docente guardado correctamente.");
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
                MessageBox.Show("Seleccione un docente de la lista.");
                return;
            }

            try
            {
                docenteNegocio.Actualizar(ObtenerDocenteDelFormulario());
                MessageBox.Show("Docente actualizado correctamente.");
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

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un docente de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este docente?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                docenteNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Docente eliminado correctamente.");
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

        private void btnLimpiar_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void frmDocentes_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void dgvDocentes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var docente = (Docente)dgvDocentes.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = docente.IdPersona;
            txtNombres.Text = docente.Nombres;
            txtApellidos.Text = docente.Apellidos;
            txtEmail.Text = docente.Email;
            txtTelefono.Text = docente.Telefono;
            txtEspecialidad.Text = docente.Especialidad;
            cboEstado.Text = docente.Estado;
        }
    }
}

