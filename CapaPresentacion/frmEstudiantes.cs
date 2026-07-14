using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Linq;
using System.Windows.Forms;
using static CapaPresentacion.Program;

namespace CapaPresentacion
{
    public partial class frmEstudiantes : Form
    {
        private EstudianteServicio estudianteNegocio = new EstudianteServicio();

        // *cambio* - Instanciamos el servicio de permisos, igual que en los otros formularios
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Almacenamos el ID del usuario activo localmente
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;

        // *cambio* - Constructor uniforme que recibe el ID de sesión
        public frmEstudiantes(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto para compatibilidad con el Diseñador de Visual Studio
        public frmEstudiantes()
        {
            InitializeComponent();
        }
        private void CargarGrid()
        {
            dgvEstudiantes.DataSource = null;
            dgvEstudiantes.DataSource = estudianteNegocio.ObtenerTodos();
        }


        private Estudiante ObtenerEstudianteDelFormulario()
        {
            return new Estudiante
            {
                IdPersona = idSeleccionado,
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Estado = cboEstado.Text,
                CodigoEstudiante = txtCodigo.Text.Trim(),
                FechaInscripcion = dtpFechaInscripcion.Value,
                Tipo = cboTipo.Text
            };
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombres.Clear();
            txtApellidos.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtCodigo.Clear();
            cboEstado.SelectedIndex = -1;
            cboTipo.SelectedIndex = -1;
            dtpFechaInscripcion.Value = DateTime.Now;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                // *cambio* - Enviamos el ID del usuario logueado a la capa de negocio
                estudianteNegocio.Guardar(ObtenerEstudianteDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Estudiante guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un estudiante de la lista.");
                return;
            }

            try
            {
                // *cambio* - Enviamos el ID del usuario logueado al actualizar
                estudianteNegocio.Actualizar(ObtenerEstudianteDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Estudiante actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un estudiante de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este estudiante?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // *cambio* - Enviamos el ID del usuario logueado al eliminar
                estudianteNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Estudiante eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void frmEstudiantes_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Bloqueo de UI consistente con el resto de pantallas
        }
        // *cambio* - Método unificado para restringir accesos usando PermisoServicio
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Evaluamos los accesos del rol para esta pantalla ("frmEstudiantes")
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmEstudiantes", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Si algo falla, bloqueamos las acciones de escritura por precaución
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void dgvEstudiantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var estudiante = (Estudiante)dgvEstudiantes.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = estudiante.IdPersona;
            txtNombres.Text = estudiante.Nombres;
            txtApellidos.Text = estudiante.Apellidos;
            txtEmail.Text = estudiante.Email;
            txtTelefono.Text = estudiante.Telefono;
            cboEstado.Text = estudiante.Estado;
            txtCodigo.Text = estudiante.CodigoEstudiante;
            dtpFechaInscripcion.Value = estudiante.FechaInscripcion;
            cboTipo.Text = estudiante.Tipo;
        }
    }
}

