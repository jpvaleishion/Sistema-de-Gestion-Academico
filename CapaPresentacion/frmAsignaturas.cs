using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmAsignaturas : Form
    {
        AsignaturaServicio asignaturaNegocio = new AsignaturaServicio();
        int idSeleccionado = 0;

        public frmAsignaturas()
        {
            InitializeComponent();
        }


        private void CargarGrid()
        {
            dgvAsignaturas.DataSource = null;
            dgvAsignaturas.DataSource = asignaturaNegocio.ObtenerTodos();
        }


        private Asignatura ObtenerAsignaturaDelFormulario()
        {
            return new Asignatura
            {
                IdAsignatura = idSeleccionado,
                Nombre = txtNombre.Text.Trim(),
                Creditos = (int)numCreditos.Value,
                Modalidad = cboModalidad.Text,
                Estado = cboEstado.Text
            };
        }


        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombre.Clear();
            numCreditos.Value = 1;
            cboModalidad.SelectedIndex = -1;
            cboEstado.SelectedIndex = -1;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                asignaturaNegocio.Guardar(ObtenerAsignaturaDelFormulario());
                MessageBox.Show("Asignatura guardada correctamente.");
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

        private void btnNuevo_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnEditar_Click_1(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione una asignatura de la lista.");
                return;
            }

            try
            {
                asignaturaNegocio.Actualizar(ObtenerAsignaturaDelFormulario());
                MessageBox.Show("Asignatura actualizada correctamente.");
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
                MessageBox.Show("Seleccione una asignatura de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar esta asignatura?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                asignaturaNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Asignatura eliminada correctamente.");
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

        private void frmAsignaturas_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void dgvAsignaturas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var asignatura = (Asignatura)dgvAsignaturas.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = asignatura.IdAsignatura;
            txtNombre.Text = asignatura.Nombre;
            numCreditos.Value = asignatura.Creditos;
            cboModalidad.Text = asignatura.Modalidad;
            cboEstado.Text = asignatura.Estado;
        }
    }
}

