using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmMatriculas : Form
    {
        MatriculaServicio matriculaNegocio = new MatriculaServicio();
        EstudianteServicio estudianteNegocio = new EstudianteServicio();
        AsignaturaServicio asignaturaNegocio = new AsignaturaServicio();
        DocenteServicio docenteNegocio = new DocenteServicio();
        CursoServicio cursoNegocio = new CursoServicio();
        PeriodoAcademicoServicio periodoNegocio = new PeriodoAcademicoServicio();

        int idSeleccionado = 0;

        public frmMatriculas()
        {
            InitializeComponent();
        }
        private void frmMatriculas_Load_1(object sender, EventArgs e)
        {
            CargarCombos();
            CargarGrid();
        }

        private void CargarCombos()
        {

            cboEstudiante.DisplayMember = "Nombres";
            cboEstudiante.ValueMember = "IdPersona";
            cboEstudiante.DataSource = estudianteNegocio.ObtenerTodos();


            cboAsignatura.DisplayMember = "Nombre";
            cboAsignatura.ValueMember = "IdAsignatura";
            cboAsignatura.DataSource = asignaturaNegocio.ObtenerTodos();


            cboDocente.DisplayMember = "Nombres";
            cboDocente.ValueMember = "IdPersona";
            cboDocente.DataSource = docenteNegocio.ObtenerTodos();


            cboCurso.DisplayMember = "NombreCurso";
            cboCurso.ValueMember = "IdCurso";
            cboCurso.DataSource = cursoNegocio.ObtenerTodos();


            cboPeriodo.DisplayMember = "NombrePeriodo";
            cboPeriodo.ValueMember = "IdPeriodo";
            cboPeriodo.DataSource = periodoNegocio.ObtenerTodos();
        }

        private void CargarGrid()
        {
            dgvMatriculas.DataSource = null;
            dgvMatriculas.DataSource = matriculaNegocio.ObtenerTodos();
        }


        private Matricula ObtenerMatriculaDelFormulario()
        {
            return new Matricula
            {
                IdMatricula = idSeleccionado,
                IdEstudiante = (int)cboEstudiante.SelectedValue,
                IdAsignatura = (int)cboAsignatura.SelectedValue,
                IdDocente = (int)cboDocente.SelectedValue,
                IdCurso = (int)cboCurso.SelectedValue,
                IdPeriodo = (int)cboPeriodo.SelectedValue,
                FechaMatricula = dtpFechaMatricula.Value,
                Estado = cboEstado.Text
            };
        }


        private void Limpiar()
        {
            idSeleccionado = 0;
            cboEstudiante.SelectedIndex = -1;
            cboAsignatura.SelectedIndex = -1;
            cboDocente.SelectedIndex = -1;
            cboCurso.SelectedIndex = -1;
            cboPeriodo.SelectedIndex = -1;
            dtpFechaMatricula.Value = DateTime.Now;
            cboEstado.SelectedIndex = -1;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                matriculaNegocio.Guardar(ObtenerMatriculaDelFormulario());
                MessageBox.Show("Matrícula guardada correctamente.");
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
                MessageBox.Show("Seleccione una matrícula de la lista.");
                return;
            }

            try
            {
                matriculaNegocio.Actualizar(ObtenerMatriculaDelFormulario());
                MessageBox.Show("Matrícula actualizada correctamente.");
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
                MessageBox.Show("Seleccione una matrícula de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar esta matrícula?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                matriculaNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Matrícula eliminada correctamente.");
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

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void dgvMatriculas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var matricula = (Matricula)dgvMatriculas.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = matricula.IdMatricula;
            cboEstudiante.SelectedValue = matricula.IdEstudiante;
            cboAsignatura.SelectedValue = matricula.IdAsignatura;
            cboDocente.SelectedValue = matricula.IdDocente;
            cboCurso.SelectedValue = matricula.IdCurso;
            cboPeriodo.SelectedValue = matricula.IdPeriodo;
            dtpFechaMatricula.Value = matricula.FechaMatricula;
            cboEstado.Text = matricula.Estado;
        }
    }
}
