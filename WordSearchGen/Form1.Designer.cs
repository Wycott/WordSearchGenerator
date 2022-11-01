namespace WordSearchGen
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
            this.Generate = new System.Windows.Forms.Button();
            this.Exit = new System.Windows.Forms.Button();
            this.FillGrid = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Generate
            // 
            this.Generate.Location = new System.Drawing.Point(271, 21);
            this.Generate.Name = "Generate";
            this.Generate.Size = new System.Drawing.Size(75, 23);
            this.Generate.TabIndex = 0;
            this.Generate.Text = "GENERATE";
            this.Generate.UseVisualStyleBackColor = true;
            this.Generate.Click += new System.EventHandler(this.Generate_Click);
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(433, 547);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(75, 23);
            this.Exit.TabIndex = 1;
            this.Exit.Text = "EXIT";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // FillGrid
            // 
            this.FillGrid.Location = new System.Drawing.Point(352, 21);
            this.FillGrid.Name = "FillGrid";
            this.FillGrid.Size = new System.Drawing.Size(75, 23);
            this.FillGrid.TabIndex = 2;
            this.FillGrid.Text = "FILL";
            this.FillGrid.UseVisualStyleBackColor = true;
            this.FillGrid.Click += new System.EventHandler(this.FillGrid_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(520, 582);
            this.Controls.Add(this.FillGrid);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.Generate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Generate;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Button FillGrid;
    }
}

