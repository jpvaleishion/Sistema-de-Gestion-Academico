using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmPeriodos : Form
    {
        PeriodoAcademicoServicio periodoNegocio = new PeriodoAcademicoServicio();
        int idSeleccionado = 0;

        public frmPeriodos()
        {
            InitializeComponent();
        }

        private void CargarGrid()
        {
            dgvPeriodos.DataSource = null;
            dgvPeriodos.DataSource = periodoNegocio.ObtenerTodos();
        }


        private PeriodoAcademico ObtenerPeriodoDelFormulario()
        {
            return new PeriodoAcademico
            {
                IdPeriodo = idSeleccionado,
                NombrePeriodo = txtNombrePeriodo.Text.Trim(),
                FechaInicio = dtpFechaInicio.Value,
                FechaFin = dtpFechaFin.Value,
                Estado = cboEstado.Text
            };
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombrePeriodo.Clear();
            dtpFechaInicio.Value = DateTime.Now;
            dtpFechaFin.Value = DateTime.Now;
            cboEstado.SelectedIndex = -1;
        }

        private void btnNuevo_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                periodoNegocio.Guardar(ObtenerPeriodoDelFormulario());
                MessageBox.Show("Período guardado correctamente.");
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

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un período de la lista.");
                return;
            }

            try
            {
                periodoNegocio.Actualizar(ObtenerPeriodoDelFormulario());
                MessageBox.Show("Período actualizado correctamente.");
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
                MessageBox.Show("Seleccione un período de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este período?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                periodoNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Período eliminado correctamente.");
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

        private void frmPeriodos_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void dgvPeriodos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var periodo = (PeriodoAcademico)dgvPeriodos.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = periodo.IdPeriodo;
            txtNombrePeriodo.Text = periodo.NombrePeriodo;
            dtpFechaInicio.Value = periodo.FechaInicio;
            dtpFechaFin.Value = periodo.FechaFin;
            cboEstado.Text = periodo.Estado;
        }
    }
}

