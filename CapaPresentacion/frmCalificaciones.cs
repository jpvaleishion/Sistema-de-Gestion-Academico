using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private BindingSource bindingSource = new BindingSource();
        private List<CalificacionDto> listaOriginalCalificaciones = new List<CalificacionDto>();
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
            cboMatricula.DisplayMember = "NombreCompleto"; // Cambia por una propiedad descriptiva si la tienes
            cboMatricula.ValueMember = "IdMatricula";
            cboMatricula.DataSource = matriculaNegocio.ObtenerParaCombo();
        }

        private void CargarGrid()
        {
            // El grid mostrará todas las columnas de la entidad Calificacion,
            // incluyendo NotaFinal y Estado que ya vienen calculados por el Negocio
            dgvCalificaciones.DataSource = null;
            //dgvCalificaciones.DataSource = calificacionNegocio.ObtenerTodos();

            List<Calificacion> calificaciones = calificacionNegocio.ObtenerTodos();

            var calificacionesDto = calificaciones.Select(calificacion => new CalificacionDto
            {
                IdCalificacion = calificacion.IdCalificacion,
                IdMatricula = calificacion.IdMatricula,
                NombreEstudiante = matriculaNegocio.ObtenerNombreEstudiantePorId(calificacion.IdMatricula),
                NombreMateria = matriculaNegocio.ObtenerNombreAsignaturaPorId(matriculaNegocio.ObtenerPorId(calificacion.IdMatricula).IdAsignatura),
                NombreCurso = matriculaNegocio.ObtenerNombreCursoPorId(matriculaNegocio.ObtenerPorId(calificacion.IdMatricula).IdCurso),
                NombreDocente = matriculaNegocio.ObtenerNombreDocentePorId(matriculaNegocio.ObtenerPorId(calificacion.IdMatricula).IdDocente),
                Nota1 = calificacion.Nota1,
                Nota2 = calificacion.Nota2,
                NotaMaxima = calificacion.NotaMaxima,
                Faltas = calificacion.Faltas,
                Observaciones = calificacion.Observaciones,
                FechaCalificacion = calificacion.FechaCalificacion
            }).ToList();

            listaOriginalCalificaciones = calificacionesDto;
            bindingSource.DataSource = listaOriginalCalificaciones;
            dgvCalificaciones.DataSource = bindingSource;

            FormatearGrid();

            //var matricula = matriculaNegocio.ObtenerPorId(calificacion.IdMatricula);
        }
        // metodo que sirve para formatear el grid y mostrar los nombres de las columnas de manera más amigable sin tanto id
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textoBusqueda = txtBuscar.Text.Trim();

                if (string.IsNullOrWhiteSpace(textoBusqueda))
                {
                    bindingSource.DataSource = listaOriginalCalificaciones;
                    return;
                }

                var columnasFiltrables = dgvCalificaciones.Columns.Cast<DataGridViewColumn>()
                    .Where(col => col.Visible && !string.IsNullOrWhiteSpace(col.DataPropertyName))
                    .ToList();

                var calificacionesFiltradas = listaOriginalCalificaciones
                    .Where(calificacion =>
                    {
                        foreach (var columna in columnasFiltrables)
                        {
                            var propiedad = calificacion.GetType().GetProperty(columna.DataPropertyName);
                            if (propiedad == null)
                            {
                                continue;
                            }

                            var valor = propiedad.GetValue(calificacion, null);
                            if (valor?.ToString()?.IndexOf(textoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                    .ToList();

                bindingSource.DataSource = calificacionesFiltradas;
            }
            catch (Exception)
            {
                bindingSource.DataSource = listaOriginalCalificaciones;
            }
        }

        private void FormatearGrid()
        {
            dgvCalificaciones.Columns["IdCalificacion"].Visible = true;
            dgvCalificaciones.Columns["IdMatricula"].Visible = false;
            dgvCalificaciones.Columns["NombreEstudiante"].HeaderText = "Estudiante";
            dgvCalificaciones.Columns["NombreMateria"].HeaderText = "Materia";
            dgvCalificaciones.Columns["NombreCurso"].HeaderText = "Curso";
            dgvCalificaciones.Columns["NombreDocente"].HeaderText = "Docente";
            dgvCalificaciones.Columns["Nota1"].HeaderText = "Nota 1";
            dgvCalificaciones.Columns["Nota2"].HeaderText = "Nota 2";
            dgvCalificaciones.Columns["NotaMaxima"].HeaderText = "Nota Máxima";
            dgvCalificaciones.Columns["Faltas"].HeaderText = "Faltas";
            dgvCalificaciones.Columns["Observaciones"].HeaderText = "Observaciones";
            dgvCalificaciones.Columns["FechaCalificacion"].HeaderText = "Fecha de Calificación";
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
        //metodo para poner los datos de la fila seleccionada en el grid en los controles del formulario para poder editarlos con doble click
        private void dgvCalificaciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var calificacion = (CalificacionDto)dgvCalificaciones.Rows[e.RowIndex].DataBoundItem;

            idSeleccionado = calificacion.IdCalificacion;
            cboMatricula.SelectedValue = calificacion.IdMatricula;
            numNota1.Value = calificacion.Nota1;
            numNota2.Value = calificacion.Nota2;
            numNotaMaxima.Value = calificacion.NotaMaxima;
            numFaltas.Value = calificacion.Faltas;
            txtObservaciones.Text = calificacion.Observaciones;
            dtpFechaCalificacion.Value = calificacion.FechaCalificacion;
        }

        private void numNota1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si no es un número y tampoco es la tecla de borrar (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Cancela la acción de la tecla
            }
        }

        private void numNota2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si no es un número y tampoco es la tecla de borrar (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Cancela la acción de la tecla
            }
        }

        private void numNotaMaxima_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si no es un número y tampoco es la tecla de borrar (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Cancela la acción de la tecla
            }
        }

        private void numFaltas_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si no es un número y tampoco es la tecla de borrar (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Cancela la acción de la tecla
            }
        }
        //clase solo visual para mostrar en el grid
        public class CalificacionDto
        {
            public int IdCalificacion { get; set; }
            public int IdMatricula { get; set; }
            public string NombreEstudiante { get; set; }
            public string NombreMateria { get; set; }
            public string NombreCurso { get; set; }
            public string NombreDocente { get; set; }
            public decimal Nota1 { get; set; }
            public decimal Nota2 { get; set; }
            public decimal NotaMaxima { get; set; }
            public int Faltas { get; set; }
            public string Observaciones { get; set; }
            public DateTime FechaCalificacion { get; set; }
            // Propiedades calculadas
            public decimal NotaFinal => (Nota1 + Nota2) / 2;
            public string Estado => NotaFinal >= 6 ? "Aprobado" : "Reprobado";
        }

        private void frmCalificaciones_Activated(object sender, EventArgs e)
        {
            CargarCombos();
        }
    }

}

