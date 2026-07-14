using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmAsignaturas : Form
    {
        private AsignaturaServicio asignaturaNegocio = new AsignaturaServicio();

        // *cambio* - Instanciamos el servicio de permisos para la validación visual
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Variable para almacenar el usuario que tiene la sesión activa
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;
        // *cambio* - Constructor recomendado: Recibe el ID del usuario desde el Menú Principal o Login
        public frmAsignaturas(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto (necesario para que el Diseñador de Visual Studio no falle)
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
                // *cambio* - Enviamos el ID del usuario logueado al guardar
                asignaturaNegocio.Guardar(ObtenerAsignaturaDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Asignatura guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // *cambio* - Enviamos el ID del usuario logueado al actualizar
                asignaturaNegocio.Actualizar(ObtenerAsignaturaDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Asignatura actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione una asignatura de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar esta asignatura?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // *cambio* - Enviamos el ID del usuario logueado al eliminar
                asignaturaNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Asignatura eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void frmAsignaturas_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Bloqueamos los botones antes de que el usuario interactúe
        }
        // *cambio* - Método encargado de activar/desactivar controles según los permisos
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Evaluamos los permisos correspondientes para esta pantalla ("frmAsignaturas")
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmAsignaturas", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Por seguridad, si falla la lectura de permisos, bloqueamos las acciones de escritura
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
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

