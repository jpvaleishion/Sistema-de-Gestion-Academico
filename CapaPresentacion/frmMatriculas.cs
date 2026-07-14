using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmMatriculas : Form
    {
        private MatriculaServicio matriculaNegocio = new MatriculaServicio();
        private EstudianteServicio estudianteNegocio = new EstudianteServicio();
        private AsignaturaServicio asignaturaNegocio = new AsignaturaServicio();
        private DocenteServicio docenteNegocio = new DocenteServicio();
        private CursoServicio cursoNegocio = new CursoServicio();
        private PeriodoAcademicoServicio periodoNegocio = new PeriodoAcademicoServicio();

        // *cambio* - Instanciamos el servicio de permisos de forma local
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Variable para retener el ID del usuario con la sesión activa
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;

        // *cambio* - Constructor que recibe el ID de usuario (Patrón Consistente)
        public frmMatriculas(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor vacío requerido por el Diseñador de Visual Studio
        public frmMatriculas()
        {
            InitializeComponent();
        }
        private void frmMatriculas_Load_1(object sender, EventArgs e)
        {
            CargarCombos();
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Aplicamos restricciones visuales al cargar
        }
        // *cambio* - Habilita o deshabilita la UI basándose en el rol del usuario logueado
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Comprobamos permisos específicos del rol para el objeto "frmMatriculas"
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmMatriculas", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Bloqueo preventivo en caso de error en la consulta de permisos
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
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
            // Control preventivo en caso de combos vacíos al intentar registrar
            if (cboEstudiante.SelectedValue == null || cboAsignatura.SelectedValue == null ||
                cboDocente.SelectedValue == null || cboCurso.SelectedValue == null || cboPeriodo.SelectedValue == null)
            {
                MessageBox.Show("Asegúrese de seleccionar todos los campos requeridos para la matrícula.", "Campos Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // *cambio* - Enviamos el ID de usuario activo a la capa de negocio
                matriculaNegocio.Guardar(ObtenerMatriculaDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Matrícula guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione una matrícula de la lista.");
                return;
            }

            try
            {
                // *cambio* - Enviamos el ID de usuario activo al actualizar
                matriculaNegocio.Actualizar(ObtenerMatriculaDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Matrícula actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // *cambio* - Enviamos el ID de usuario activo al eliminar
                matriculaNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Matrícula eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
