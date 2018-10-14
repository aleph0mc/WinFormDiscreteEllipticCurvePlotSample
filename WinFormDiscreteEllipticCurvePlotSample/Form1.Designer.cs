namespace WinFormDiscreteEllipticCurvePlotSample
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
            this.txtLoops = new System.Windows.Forms.TextBox();
            this.lblSk = new System.Windows.Forms.Label();
            this.btnGen = new System.Windows.Forms.Button();
            this.lblHrLine = new System.Windows.Forms.Label();
            this.pbEcDescrete = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbEcDescrete)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLoops
            // 
            this.txtLoops.Location = new System.Drawing.Point(142, 12);
            this.txtLoops.Name = "txtLoops";
            this.txtLoops.Size = new System.Drawing.Size(134, 20);
            this.txtLoops.TabIndex = 0;
            // 
            // lblSk
            // 
            this.lblSk.AutoSize = true;
            this.lblSk.Location = new System.Drawing.Point(12, 15);
            this.lblSk.Name = "lblSk";
            this.lblSk.Size = new System.Drawing.Size(126, 13);
            this.lblSk.TabIndex = 1;
            this.lblSk.Text = "Loop Count (max 10,000)";
            // 
            // btnGen
            // 
            this.btnGen.Location = new System.Drawing.Point(297, 12);
            this.btnGen.Name = "btnGen";
            this.btnGen.Size = new System.Drawing.Size(119, 23);
            this.btnGen.TabIndex = 2;
            this.btnGen.Text = "Generate";
            this.btnGen.UseVisualStyleBackColor = true;
            this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
            // 
            // lblHrLine
            // 
            this.lblHrLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblHrLine.Location = new System.Drawing.Point(12, 49);
            this.lblHrLine.Name = "lblHrLine";
            this.lblHrLine.Size = new System.Drawing.Size(600, 2);
            this.lblHrLine.TabIndex = 3;
            // 
            // pbEcDescrete
            // 
            this.pbEcDescrete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbEcDescrete.Location = new System.Drawing.Point(15, 54);
            this.pbEcDescrete.Name = "pbEcDescrete";
            this.pbEcDescrete.Size = new System.Drawing.Size(500, 500);
            this.pbEcDescrete.TabIndex = 4;
            this.pbEcDescrete.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 561);
            this.Controls.Add(this.pbEcDescrete);
            this.Controls.Add(this.lblHrLine);
            this.Controls.Add(this.btnGen);
            this.Controls.Add(this.lblSk);
            this.Controls.Add(this.txtLoops);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pbEcDescrete)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLoops;
        private System.Windows.Forms.Label lblSk;
        private System.Windows.Forms.Button btnGen;
        private System.Windows.Forms.Label lblHrLine;
        private System.Windows.Forms.PictureBox pbEcDescrete;
    }
}

