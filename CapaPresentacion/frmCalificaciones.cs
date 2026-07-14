using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmCalificaciones : Form
    {
        private CalificacionServicio calificacionNegocio = new CalificacionServicio();
        private MatriculaServicio matriculaNegocio = new MatriculaServicio();

        // *cambio* - Instanciamos el servicio de permisos
        private PermisoServicio permisoNegocio = new PermisoServicio();

        // *cambio* - Almacenamos el ID del usuario logueado en esta sesión
        private int idUsuarioLogueado;
        private int idSeleccionado = 0;
        // *cambio* - Constructor recomendado: Recibe el ID del usuario logueado
        public frmCalificaciones(int idUsuario)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuario;
        }
        // Constructor por defecto para que no falle el Diseñador de Visual Studio
        public frmCalificaciones()
        {
            InitializeComponent();
        }

        private void CargarCombos()
        {
            // El combo muestra las matrículas disponibles para asignar la nota
            cboMatricula.DisplayMember = "IdMatricula"; // Cambia por una propiedad descriptiva si la tienes
            cboMatricula.ValueMember = "IdMatricula";
            cboMatricula.DataSource = matriculaNegocio.ObtenerTodos();
        }

        private void CargarGrid()
        {
            // El grid mostrará todas las columnas de la entidad Calificacion,
            // incluyendo NotaFinal y Estado que ya vienen calculados por el Negocio
            dgvCalificaciones.DataSource = null;
            dgvCalificaciones.DataSource = calificacionNegocio.ObtenerTodos();
        }


        // La presentación solo envía los datos capturados.
        // NotaFinal y Estado los calcula el Negocio antes de guardar (RN-06 y RN-07).
        private Calificacion ObtenerCalificacionDelFormulario()
        {
            return new Calificacion
            {
                IdCalificacion = idSeleccionado,
                IdMatricula = (int)cboMatricula.SelectedValue,
                Nota1 = (decimal)numNota1.Value,
                Nota2 = (decimal)numNota2.Value,
                NotaMaxima = (decimal)numNotaMaxima.Value,
                Faltas = (int)numFaltas.Value,
                Observaciones = txtObservaciones.Text.Trim(),
                FechaCalificacion = dtpFechaCalificacion.Value
            };
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            cboMatricula.SelectedIndex = 0;
            numNota1.Value = 0;
            numNota2.Value = 0;
            numNotaMaxima.Value = 10;
            numFaltas.Value = 0;
            txtObservaciones.Clear();
            dtpFechaCalificacion.Value = DateTime.Now;
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                // *cambio* - Enviamos el ID del usuario activo para validar permisos en negocio y registrar bitácora
                calificacionNegocio.Guardar(ObtenerCalificacionDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Calificación guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione una calificación de la lista.");
                return;
            }

            try
            {
                // *cambio* - Enviamos el ID del usuario activo para actualizar
                calificacionNegocio.Actualizar(ObtenerCalificacionDelFormulario(), idUsuarioLogueado);
                MessageBox.Show("Calificación actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione una calificación de la lista.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar esta calificación?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // *cambio* - Enviamos el ID del usuario activo para eliminar
                calificacionNegocio.Eliminar(idSeleccionado, idUsuarioLogueado);
                MessageBox.Show("Calificación eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void frmCalificaciones_Load_1(object sender, EventArgs e)
        {
            CargarCombos();
            CargarGrid();
            AplicarPermisosVisuales(); // *cambio* - Validamos los permisos al abrir la pantalla
        }
        // *cambio* - Método para restringir los botones según el rol del usuario
        private void AplicarPermisosVisuales()
        {
            try
            {
                // Comprobamos permisos específicos para la pantalla de "frmCalificaciones"
                btnGuardar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Crear");
                btnEditar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Modificar");
                btnEliminar.Enabled = permisoNegocio.TienePermiso(idUsuarioLogueado, "frmCalificaciones", "Eliminar");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los permisos de seguridad: " + ex.Message, "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Si hay un error, deshabilitamos todo por seguridad
                btnGuardar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }
        private void dgvCalificaciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var calificacion = (Calificacion)dgvCalificaciones.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = calificacion.IdCalificacion;
            cboMatricula.SelectedValue = calificacion.IdMatricula;
            numNota1.Value = (decimal)calificacion.Nota1;
            numNota2.Value = (decimal)calificacion.Nota2;
            numNotaMaxima.Value = (decimal)calificacion.NotaMaxima;
            numFaltas.Value = calificacion.Faltas;
            txtObservaciones.Text = calificacion.Observaciones;
            dtpFechaCalificacion.Value = calificacion.FechaCalificacion;
        }
    }
}

