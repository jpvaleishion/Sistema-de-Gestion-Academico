using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmDocentes : Form
    {
        private DocenteServicio docenteNegocio = new DocenteServicio();

        // *cambio* - Instanciamos el servicio de permisos para el control visual de la UI
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Almacenamos el ID del usuario con sesión activa
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;
        private BindingSource bindingSource = new BindingSource();
        private List<Docente> listaOriginalDocentes = new List<Docente>();

        // *cambio* - Constructor recomendado: Recibe el ID de sesión del usuario logueado
        public frmDocentes(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto para compatibilidad con el Diseñador de Visual Studio
        public frmDocentes()
        {
            InitializeComponent();
        }
        private void CargarGrid()
        {
            listaOriginalDocentes = docenteNegocio.ObtenerTodos();
            bindingSource.DataSource = listaOriginalDocentes;
            dgvDocentes.DataSource = bindingSource;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textoBusqueda = txtBuscar.Text.Trim();

                if (string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    bindingSource.DataSource = listaOriginalDocentes;
                    return;
                }

                var columnasFiltrables = dgvDocentes.Columns.Cast<DataGridViewColumn>()
                    .Where(col => col.Visible && !string.IsNullOrWhiteSpace(col.DataPropertyName))
                    .ToList();

                var docentesFiltrados = listaOriginalDocentes
                    .Where(docente =>
                    {
                        foreach (var columna in columnasFiltrables)
                        {
                            var propiedad = docente.GetType().GetProperty(columna.DataPropertyName);
                            if (propiedad == null)
                            {
                                continue;
                            }

                            var valor = propiedad.GetValue(docente, null);
                            if (valor?.ToString()?.IndexOf(textoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                    .ToList();

                bindingSource.DataSource = docentesFiltrados;
            }
            catch (Exception)
            {
                bindingSource.DataSource = listaOriginalDocentes;
            }
        }

        private Docente ObtenerDocenteDelFormulario()
        {
            return new Docente
            {
                IdPersona = idSeleccionado,
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Especialidad = txtEspecialidad.Text.Trim(),
                Estado = cboEstado.Text
            };
        }



        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombres.Clear();
            txtApellidos.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtEspecialidad.Clear();
            cboEstado.SelectedIndex = -1;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                // *cambio* - Enviamos el ID del usuario activo al guardar para validar permisos y registrar bitácora
                docenteNegocio.Guardar(ObtenerDocenteDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Docente guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un docente de la lista.");
                return;
            }

            try
            {
                // *cambio* - Enviamos el ID del usuario activo al actualizar
                docenteNegocio.Actualizar(ObtenerDocenteDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Docente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione un docente de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este docente?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // *cambio* - Enviamos el ID del usuario activo al eliminar
                docenteNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Docente eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void frmDocentes_Load_1(object sender, EventArgs e)
        {
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Bloqueo preventivo de controles según permisos
        }
        // *cambio* - Método que habilita/deshabilita los botones según el rol del usuario
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Verificamos los permisos asignados a esta pantalla ("frmDocentes")
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmDocentes", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmDocentes", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmDocentes", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // En caso de fallo por seguridad bloqueamos las acciones de escritura
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }
        private void dgvDocentes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var docente = (Docente)dgvDocentes.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = docente.IdPersona;
            txtNombres.Text = docente.Nombres;
            txtApellidos.Text = docente.Apellidos;
            txtEmail.Text = docente.Email;
            txtTelefono.Text = docente.Telefono;
            txtEspecialidad.Text = docente.Especialidad;
            cboEstado.Text = docente.Estado;
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si no es un número y tampoco es la tecla de borrar (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Cancela la acción de la tecla
            }
        }
    }
}

