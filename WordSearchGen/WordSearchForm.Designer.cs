namespace WordSearchGen
{
    sealed partial class WordSearchForm
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
            this.SkillLevelGroup = new System.Windows.Forms.GroupBox();
            this.Beginner = new System.Windows.Forms.RadioButton();
            this.Intermediate = new System.Windows.Forms.RadioButton();
            this.Advanced = new System.Windows.Forms.RadioButton();
            this.SkillLevelGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // Generate
            // 
            this.Generate.Location = new System.Drawing.Point(12, 56);
            this.Generate.Name = "Generate";
            this.Generate.Size = new System.Drawing.Size(75, 23);
            this.Generate.TabIndex = 0;
            this.Generate.Text = "GENERATE";
            this.Generate.UseVisualStyleBackColor = true;
            this.Generate.Click += new System.EventHandler(this.Generate_Click);
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(174, 56);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(75, 23);
            this.Exit.TabIndex = 1;
            this.Exit.Text = "EXIT";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // FillGrid
            // 
            this.FillGrid.Location = new System.Drawing.Point(93, 56);
            this.FillGrid.Name = "FillGrid";
            this.FillGrid.Size = new System.Drawing.Size(75, 23);
            this.FillGrid.TabIndex = 2;
            this.FillGrid.Text = "FILL";
            this.FillGrid.UseVisualStyleBackColor = true;
            this.FillGrid.Click += new System.EventHandler(this.FillGrid_Click);
            // 
            // SkillLevelGroup
            // 
            this.SkillLevelGroup.Controls.Add(this.Beginner);
            this.SkillLevelGroup.Controls.Add(this.Intermediate);
            this.SkillLevelGroup.Controls.Add(this.Advanced);
            this.SkillLevelGroup.Location = new System.Drawing.Point(12, 8);
            this.SkillLevelGroup.Name = "SkillLevelGroup";
            this.SkillLevelGroup.Size = new System.Drawing.Size(250, 42);
            this.SkillLevelGroup.TabIndex = 3;
            this.SkillLevelGroup.TabStop = false;
            this.SkillLevelGroup.Text = "Skill level";
            // 
            // Beginner
            // 
            this.Beginner.AutoSize = true;
            this.Beginner.Location = new System.Drawing.Point(8, 17);
            this.Beginner.Name = "Beginner";
            this.Beginner.TabIndex = 0;
            this.Beginner.Text = "Beginner";
            this.Beginner.UseVisualStyleBackColor = true;
            // 
            // Intermediate
            // 
            this.Intermediate.AutoSize = true;
            this.Intermediate.Location = new System.Drawing.Point(80, 17);
            this.Intermediate.Name = "Intermediate";
            this.Intermediate.TabIndex = 1;
            this.Intermediate.Text = "Intermediate";
            this.Intermediate.UseVisualStyleBackColor = true;
            // 
            // Advanced
            // 
            this.Advanced.AutoSize = true;
            this.Advanced.Checked = true;
            this.Advanced.Location = new System.Drawing.Point(170, 17);
            this.Advanced.Name = "Advanced";
            this.Advanced.TabIndex = 2;
            this.Advanced.TabStop = true;
            this.Advanced.Text = "Advanced";
            this.Advanced.UseVisualStyleBackColor = true;
            // 
            // WordSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(520, 582);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Controls.Add(this.SkillLevelGroup);
            this.Controls.Add(this.FillGrid);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.Generate);
            this.Name = "WordSearchForm";
            this.Text = "Word Search Generator";
            this.SkillLevelGroup.ResumeLayout(false);
            this.SkillLevelGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Generate;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Button FillGrid;
        private System.Windows.Forms.GroupBox SkillLevelGroup;
        private System.Windows.Forms.RadioButton Beginner;
        private System.Windows.Forms.RadioButton Intermediate;
        private System.Windows.Forms.RadioButton Advanced;
    }
}

