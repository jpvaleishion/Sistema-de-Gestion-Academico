    namespace CapaPresentacion
    {
        partial class frmPrincipal
        {
            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            #region Windows Form Designer generated code

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuMantenimiento = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocentes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEstudiantes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAsignaturas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCursos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPeriodos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProcesos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMatriculas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCalificaciones = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdministracion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblUsuarioActivo = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMantenimiento,
            this.mnuProcesos,
            this.mnuAdministracion,
            this.mnuSalir});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1209, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuMantenimiento
            // 
            this.mnuMantenimiento.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDocentes,
            this.mnuEstudiantes,
            this.mnuAsignaturas,
            this.mnuCursos,
            this.mnuPeriodos});
            this.mnuMantenimiento.Name = "mnuMantenimiento";
            this.mnuMantenimiento.Size = new System.Drawing.Size(124, 24);
            this.mnuMantenimiento.Text = "Mantenimiento";
            // 
            // mnuDocentes
            // 
            this.mnuDocentes.Name = "mnuDocentes";
            this.mnuDocentes.Size = new System.Drawing.Size(169, 26);
            this.mnuDocentes.Text = "Docente";
            this.mnuDocentes.Click += new System.EventHandler(this.mnuDocentes_Click);
            // 
            // mnuEstudiantes
            // 
            this.mnuEstudiantes.Name = "mnuEstudiantes";
            this.mnuEstudiantes.Size = new System.Drawing.Size(169, 26);
            this.mnuEstudiantes.Text = "Estudiante";
            this.mnuEstudiantes.Click += new System.EventHandler(this.mnuEstudiantes_Click);
            // 
            // mnuAsignaturas
            // 
            this.mnuAsignaturas.Name = "mnuAsignaturas";
            this.mnuAsignaturas.Size = new System.Drawing.Size(169, 26);
            this.mnuAsignaturas.Text = "Asignaturas";
            this.mnuAsignaturas.Click += new System.EventHandler(this.mnuAsignaturas_Click);
            // 
            // mnuCursos
            // 
            this.mnuCursos.Name = "mnuCursos";
            this.mnuCursos.Size = new System.Drawing.Size(169, 26);
            this.mnuCursos.Text = "Cursos";
            this.mnuCursos.Click += new System.EventHandler(this.mnuCursos_Click);
            // 
            // mnuPeriodos
            // 
            this.mnuPeriodos.Name = "mnuPeriodos";
            this.mnuPeriodos.Size = new System.Drawing.Size(169, 26);
            this.mnuPeriodos.Text = "Períodos";
            this.mnuPeriodos.Click += new System.EventHandler(this.mnuPeriodos_Click);
            // 
            // mnuProcesos
            // 
            this.mnuProcesos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMatriculas,
            this.mnuCalificaciones});
            this.mnuProcesos.Name = "mnuProcesos";
            this.mnuProcesos.Size = new System.Drawing.Size(81, 24);
            this.mnuProcesos.Text = "Procesos";
            // 
            // mnuMatriculas
            // 
            this.mnuMatriculas.Name = "mnuMatriculas";
            this.mnuMatriculas.Size = new System.Drawing.Size(183, 26);
            this.mnuMatriculas.Text = "Matrículas";
            this.mnuMatriculas.Click += new System.EventHandler(this.mnuMatriculas_Click);
            // 
            // mnuCalificaciones
            // 
            this.mnuCalificaciones.Name = "mnuCalificaciones";
            this.mnuCalificaciones.Size = new System.Drawing.Size(183, 26);
            this.mnuCalificaciones.Text = "Calificaciones";
            this.mnuCalificaciones.Click += new System.EventHandler(this.mnuCalificaciones_Click);
            // 
            // mnuAdministracion
            // 
            this.mnuAdministracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUsuarios});
            this.mnuAdministracion.Name = "mnuAdministracion";
            this.mnuAdministracion.Size = new System.Drawing.Size(123, 24);
            this.mnuAdministracion.Text = "Administracion";
            // 
            // mnuUsuarios
            // 
            this.mnuUsuarios.Name = "mnuUsuarios";
            this.mnuUsuarios.Size = new System.Drawing.Size(148, 26);
            this.mnuUsuarios.Text = "Usuarios";
            this.mnuUsuarios.Click += new System.EventHandler(this.mnuUsuarios_Click);
            // 
            // mnuSalir
            // 
            this.mnuSalir.Name = "mnuSalir";
            this.mnuSalir.Size = new System.Drawing.Size(52, 24);
            this.mnuSalir.Text = "Salir";
            this.mnuSalir.Click += new System.EventHandler(this.mnuSalir_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuarioActivo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 742);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1209, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblUsuarioActivo
            // 
            this.lblUsuarioActivo.Name = "lblUsuarioActivo";
            this.lblUsuarioActivo.Size = new System.Drawing.Size(151, 20);
            this.lblUsuarioActivo.Text = "toolStripStatusLabel1";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 768);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPrincipal";
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.MenuStrip menuStrip1;
            private System.Windows.Forms.ToolStripMenuItem mnuMantenimiento;
            private System.Windows.Forms.ToolStripMenuItem mnuDocentes;
            private System.Windows.Forms.ToolStripMenuItem mnuEstudiantes;
            private System.Windows.Forms.ToolStripMenuItem mnuAsignaturas;
            private System.Windows.Forms.ToolStripMenuItem mnuCursos;
            private System.Windows.Forms.ToolStripMenuItem mnuPeriodos;
            private System.Windows.Forms.ToolStripMenuItem mnuProcesos;
            private System.Windows.Forms.ToolStripMenuItem mnuMatriculas;
            private System.Windows.Forms.ToolStripMenuItem mnuCalificaciones;
            private System.Windows.Forms.ToolStripMenuItem mnuAdministracion;
            private System.Windows.Forms.ToolStripMenuItem mnuUsuarios;
            private System.Windows.Forms.ToolStripMenuItem mnuSalir;
            private System.Windows.Forms.StatusStrip statusStrip1;
            private System.Windows.Forms.ToolStripStatusLabel lblUsuarioActivo;
        }
    }