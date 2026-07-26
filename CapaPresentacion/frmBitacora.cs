using CapaEntidades.Entidades;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmBitacora : Form
    {
        private readonly BitacoraServicio bitacoraServicio = new BitacoraServicio();
        private List<Bitacora> listaBitacora = new List<Bitacora>();

        public frmBitacora()
        {
            InitializeComponent();
        }

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            listaBitacora = bitacoraServicio.ObtenerTodo()
                .OrderByDescending(x => x.FechaHora)
                .ToList();

            dgvBitacora.DataSource = listaBitacora;

            if (dgvBitacora.Columns.Contains("FechaHora"))
            {
                dgvBitacora.Columns["FechaHora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgvBitacora.DataSource = listaBitacora;
            }
            else
            {
                var filtrados = listaBitacora.Where(x =>
                    (x.Modulo != null && x.Modulo.ToLower().Contains(filtro)) ||
                    (x.Accion != null && x.Accion.ToLower().Contains(filtro)) ||
                    (x.Descripcion != null && x.Descripcion.ToLower().Contains(filtro)) ||
                    (x.NombreUsuario != null && x.NombreUsuario.ToLower().Contains(filtro)) ||
                    x.IdUsuario.ToString().Contains(filtro)
                ).ToList();

                dgvBitacora.DataSource = filtrados;
            }
        }

    }
}
