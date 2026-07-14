using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmCursos : Form
    {
        private CursoServicio cursoNegocio = new CursoServicio();

        // *cambio* - Instanciamos el servicio de permisos
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Variable para guardar el usuario activo
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;
        // *cambio* - Constructor recomendado para pasar el ID del usuario
        public frmCursos(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto para el diseñador de Visual Studio
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
                // *cambio* - Enviamos el ID del usuario activo al guardar
                cursoNegocio.Guardar(ObtenerCursoDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Curso guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un curso de la lista.");
                return;
            }

            try
            {
                // *cambio* - Enviamos el ID del usuario activo al actualizar
                cursoNegocio.Actualizar(ObtenerCursoDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Curso actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un curso de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este curso?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // *cambio* - Enviamos el ID del usuario activo al eliminar
                cursoNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Curso eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void frmCursos_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Bloqueo preventivo de interfaz según permisos
        }
        // *cambio* - Método encargado de ajustar la UI según los privilegios del rol
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Comprobamos permisos asignados al formulario "frmCursos"
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmCursos", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmCursos", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmCursos", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar las credenciales de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Si la verificación falla, se deniegan las operaciones por precaución
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
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
