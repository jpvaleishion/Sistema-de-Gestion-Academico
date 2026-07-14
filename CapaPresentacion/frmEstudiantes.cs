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
        EstudianteServicio estudianteNegocio = new EstudianteServicio();
        int idSeleccionado = 0;

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
                estudianteNegocio.Guardar(ObtenerEstudianteDelFormulario());
                MessageBox.Show("Estudiante guardado correctamente.");
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
                MessageBox.Show("Seleccione un estudiante de la lista.");
                return;
            }

            try
            {
                estudianteNegocio.Actualizar(ObtenerEstudianteDelFormulario());
                MessageBox.Show("Estudiante actualizado correctamente.");
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
                MessageBox.Show("Seleccione un estudiante de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este estudiante?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                estudianteNegocio.Eliminar(idSeleccionado);
                MessageBox.Show("Estudiante eliminado correctamente.");
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
            // 1. Cargamos los datos de los estudiantes en la tabla (tu código actual)
            CargarGrid();

            // 2. *cambio* - Buscamos en la sesión los permisos específicos para esta pantalla
            // (Asegúrate de tener "using System.Linq;" al inicio de tu archivo)
            var permiso = SesionActual.Permisos.FirstOrDefault(p => p.NombreFormulario == "frmEstudiantes");

            if (permiso != null)
            {
                // Habilitamos o deshabilitamos los botones de tu formulario según la base de datos
                btnGuardar.Enabled = permiso.Crear;
                btnEditar.Enabled = permiso.Modificar;
                btnEliminar.Enabled = permiso.Eliminar;
            }
            else
            {
                // Medida de seguridad: Si el rol no tiene este formulario asignado, bloqueamos todo
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

