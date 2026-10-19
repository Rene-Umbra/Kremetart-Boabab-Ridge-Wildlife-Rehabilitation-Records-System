namespace Kremetart_Ridge_WinForm_Project
{
    partial class Form1
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
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvAnimals = new System.Windows.Forms.DataGridView();
            this.btnGenerateSummary = new System.Windows.Forms.Button();
            this.txtSummaryOutput = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnimals)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.Location = new System.Drawing.Point(532, 375);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(101, 26);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "Delete Animal";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvAnimals
            // 
            this.dgvAnimals.AllowUserToAddRows = false;
            this.dgvAnimals.AllowUserToDeleteRows = false;
            this.dgvAnimals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAnimals.Location = new System.Drawing.Point(34, 37);
            this.dgvAnimals.MultiSelect = false;
            this.dgvAnimals.Name = "dgvAnimals";
            this.dgvAnimals.ReadOnly = true;
            this.dgvAnimals.RowTemplate.Height = 24;
            this.dgvAnimals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAnimals.Size = new System.Drawing.Size(380, 262);
            this.dgvAnimals.TabIndex = 1;
            this.dgvAnimals.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAnimals_CellContentClick);
            // 
            // btnGenerateSummary
            // 
            this.btnGenerateSummary.AutoSize = true;
            this.btnGenerateSummary.Location = new System.Drawing.Point(532, 415);
            this.btnGenerateSummary.Name = "btnGenerateSummary";
            this.btnGenerateSummary.Size = new System.Drawing.Size(140, 26);
            this.btnGenerateSummary.TabIndex = 2;
            this.btnGenerateSummary.Text = "Generate Summary";
            this.btnGenerateSummary.UseVisualStyleBackColor = true;
            this.btnGenerateSummary.Click += new System.EventHandler(this.btnGenerateSummary_Click);
            // 
            // txtSummaryOutput
            // 
            this.txtSummaryOutput.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtSummaryOutput.Location = new System.Drawing.Point(450, 37);
            this.txtSummaryOutput.Multiline = true;
            this.txtSummaryOutput.Name = "txtSummaryOutput";
            this.txtSummaryOutput.ReadOnly = true;
            this.txtSummaryOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSummaryOutput.Size = new System.Drawing.Size(320, 320);
            this.txtSummaryOutput.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtSummaryOutput);
            this.Controls.Add(this.btnGenerateSummary);
            this.Controls.Add(this.dgvAnimals);
            this.Controls.Add(this.btnDelete);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnimals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridView dgvAnimals;
        private System.Windows.Forms.Button btnGenerateSummary;
        private System.Windows.Forms.TextBox txtSummaryOutput;
    }
}
