namespace CapaPresentacion
{
    partial class frmCalificaciones
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
            this.cboMatricula = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpFechaCalificacion = new System.Windows.Forms.DateTimePicker();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvCalificaciones = new System.Windows.Forms.DataGridView();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.numNota1 = new System.Windows.Forms.NumericUpDown();
            this.numNota2 = new System.Windows.Forms.NumericUpDown();
            this.numNotaMaxima = new System.Windows.Forms.NumericUpDown();
            this.numFaltas = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalificaciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNota1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNota2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNotaMaxima)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFaltas)).BeginInit();
            this.SuspendLayout();
            // 
            // cboMatricula
            // 
            this.cboMatricula.FormattingEnabled = true;
            this.cboMatricula.Location = new System.Drawing.Point(349, 406);
            this.cboMatricula.Margin = new System.Windows.Forms.Padding(4);
            this.cboMatricula.Name = "cboMatricula";
            this.cboMatricula.Size = new System.Drawing.Size(232, 24);
            this.cboMatricula.TabIndex = 136;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(113, 658);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(186, 29);
            this.label7.TabIndex = 134;
            this.label7.Text = "F_Calificacion:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(104, 618);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 29);
            this.label6.TabIndex = 133;
            this.label6.Text = "Observaciones:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(196, 568);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 29);
            this.label4.TabIndex = 132;
            this.label4.Text = "Faltas:";
            // 
            // dtpFechaCalificacion
            // 
            this.dtpFechaCalificacion.Location = new System.Drawing.Point(349, 665);
            this.dtpFechaCalificacion.Name = "dtpFechaCalificacion";
            this.dtpFechaCalificacion.Size = new System.Drawing.Size(232, 22);
            this.dtpFechaCalificacion.TabIndex = 131;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(785, 625);
            this.btnLimpiar.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(179, 47);
            this.btnLimpiar.TabIndex = 130;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click_1);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(785, 568);
            this.btnEliminar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(179, 47);
            this.btnEliminar.TabIndex = 129;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click_1);
            // 
            // btnEditar
            // 
            this.btnEditar.Location = new System.Drawing.Point(785, 508);
            this.btnEditar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(179, 47);
            this.btnEditar.TabIndex = 128;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click_1);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(785, 445);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(179, 47);
            this.btnGuardar.TabIndex = 127;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click_1);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(785, 388);
            this.btnNuevo.Margin = new System.Windows.Forms.Padding(4);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(179, 47);
            this.btnNuevo.TabIndex = 126;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(138, 528);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(171, 29);
            this.label5.TabIndex = 124;
            this.label5.Text = "Nota Maxima:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(213, 490);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 29);
            this.label3.TabIndex = 123;
            this.label3.Text = "Nota 2:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.No;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(213, 444);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 29);
            this.label2.TabIndex = 122;
            this.label2.Text = "Nota 1:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(166, 406);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 29);
            this.label1.TabIndex = 121;
            this.label1.Text = "Estudiante:";
            // 
            // dgvCalificaciones
            // 
            this.dgvCalificaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCalificaciones.Location = new System.Drawing.Point(131, 92);
            this.dgvCalificaciones.Margin = new System.Windows.Forms.Padding(4);
            this.dgvCalificaciones.Name = "dgvCalificaciones";
            this.dgvCalificaciones.RowHeadersWidth = 51;
            this.dgvCalificaciones.Size = new System.Drawing.Size(932, 262);
            this.dgvCalificaciones.TabIndex = 120;
            this.dgvCalificaciones.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCalificaciones_CellDoubleClick);
            // 
            // txtObservaciones
            // 
            this.txtObservaciones.Location = new System.Drawing.Point(349, 625);
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(232, 22);
            this.txtObservaciones.TabIndex = 141;
            // 
            // numNota1
            // 
            this.numNota1.Location = new System.Drawing.Point(349, 452);
            this.numNota1.Name = "numNota1";
            this.numNota1.Size = new System.Drawing.Size(232, 22);
            this.numNota1.TabIndex = 142;
            // 
            // numNota2
            // 
            this.numNota2.Location = new System.Drawing.Point(349, 497);
            this.numNota2.Name = "numNota2";
            this.numNota2.Size = new System.Drawing.Size(232, 22);
            this.numNota2.TabIndex = 143;
            // 
            // numNotaMaxima
            // 
            this.numNotaMaxima.Location = new System.Drawing.Point(349, 536);
            this.numNotaMaxima.Name = "numNotaMaxima";
            this.numNotaMaxima.Size = new System.Drawing.Size(232, 22);
            this.numNotaMaxima.TabIndex = 144;
            // 
            // numFaltas
            // 
            this.numFaltas.Location = new System.Drawing.Point(349, 576);
            this.numFaltas.Name = "numFaltas";
            this.numFaltas.Size = new System.Drawing.Size(232, 22);
            this.numFaltas.TabIndex = 145;
            // 
            // frmCalificaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1190, 781);
            this.Controls.Add(this.numFaltas);
            this.Controls.Add(this.numNotaMaxima);
            this.Controls.Add(this.numNota2);
            this.Controls.Add(this.numNota1);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.cboMatricula);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtpFechaCalificacion);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCalificaciones);
            this.Name = "frmCalificaciones";
            this.Text = "frmCalificaciones";
            this.Load += new System.EventHandler(this.frmCalificaciones_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalificaciones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNota1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNota2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNotaMaxima)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFaltas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cboMatricula;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpFechaCalificacion;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvCalificaciones;
        private System.Windows.Forms.TextBox txtObservaciones;
        private System.Windows.Forms.NumericUpDown numNota1;
        private System.Windows.Forms.NumericUpDown numNota2;
        private System.Windows.Forms.NumericUpDown numNotaMaxima;
        private System.Windows.Forms.NumericUpDown numFaltas;
    }
}