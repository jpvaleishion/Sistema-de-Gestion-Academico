using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmPeriodos : Form
    {
        private PeriodoAcademicoServicio periodoNegocio = new PeriodoAcademicoServicio();

        // *cambio* - Instanciamos el servicio de permisos de forma local
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Variable para retener el ID del usuario activo
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;

        // *cambio* - Constructor que recibe el ID de usuario (Patrón Estándar)
        public frmPeriodos(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto para compatibilidad con el Diseñador de Visual Studio
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
                // *cambio* - Pasamos el ID del usuario que registra el período
                periodoNegocio.Guardar(ObtenerPeriodoDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Período guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un período de la lista.");
                return;
            }

            try
            {
                // *cambio* - Pasamos el ID del usuario que actualiza el período
                periodoNegocio.Actualizar(ObtenerPeriodoDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Período actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un período de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este período?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // *cambio* - Pasamos el ID del usuario que elimina el período
                periodoNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Período eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnLimpiar_Click_1(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void frmPeriodos_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Bloqueo de UI consistente según permisos del rol
        }
        // *cambio* - Método unificado para restringir accesos usando PermisoServicio
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Evaluamos los accesos asignados a esta pantalla ("frmPeriodos")
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmPeriodos", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmPeriodos", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmPeriodos", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Si la verificación falla, bloqueamos los botones de escritura por seguridad
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
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

