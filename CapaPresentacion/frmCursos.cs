using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmCursos : Form
    {
        CursoServicio cursoNegocio = new CursoServicio();
        int idSeleccionado = 0;

        public frmCursos()
        {
            InitializeComponent();
        }


        private void CargarGrid()
        {
            dgvCursos.DataSource = null;
            dgvCursos.DataSource = cursoNegocio.ObtenerTodos();
        }

        private Curso ObtenerCursoDelFormulario()
        {
            return new Curso
            {
                IdCurso = idSeleccionado,
                NombreCurso = txtNombreCurso.Text.Trim(),
                Paralelo = txtParalelo.Text.Trim(),
                Capacidad = (int)numCapacidad.Value,
                Estado = cboEstado.Text
            };
        }



        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombreCurso.Clear();
            txtParalelo.Clear();
            numCapacidad.Value = 1;
            cboEstado.SelectedIndex = -1;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                cursoNegocio.Guardar(ObtenerCursoDelFormulario());
                MessageBox.Show("Curso guardado correctamente.");
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
                MessageBox.Show("Seleccione un curso de la lista.");
                return;
            }

            try
            {
                cursoNegocio.Actualizar(ObtenerCursoDelFormulario());
                MessageBox.Show("Curso actualizado correctamente.");
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
                MessageBox.Show("Seleccione un curso de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este curso?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                cursoNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Curso eliminado correctamente.");
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

        private void frmCursos_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void dgvCursos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var curso = (Curso)dgvCursos.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = curso.IdCurso;
            txtNombreCurso.Text = curso.NombreCurso;
            txtParalelo.Text = curso.Paralelo;
            numCapacidad.Value = curso.Capacidad;
            cboEstado.Text = curso.Estado;
        }
    }
}
