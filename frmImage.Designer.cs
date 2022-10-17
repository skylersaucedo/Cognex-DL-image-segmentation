namespace TSI_DL_cognexIntegration
{
    partial class frmImage
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblImgPath = new System.Windows.Forms.Label();
            this.txbxModelText = new System.Windows.Forms.TextBox();
            this.tBarResults = new System.Windows.Forms.TrackBar();
            this.lblNumber = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarResults)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(271, 281);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblImgPath
            // 
            this.lblImgPath.AutoSize = true;
            this.lblImgPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImgPath.Location = new System.Drawing.Point(9, 9);
            this.lblImgPath.Name = "lblImgPath";
            this.lblImgPath.Size = new System.Drawing.Size(130, 16);
            this.lblImgPath.TabIndex = 1;
            this.lblImgPath.Text = "IMAGE PATH HERE";
            // 
            // txbxModelText
            // 
            this.txbxModelText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbxModelText.Location = new System.Drawing.Point(289, 28);
            this.txbxModelText.Multiline = true;
            this.txbxModelText.Name = "txbxModelText";
            this.txbxModelText.Size = new System.Drawing.Size(262, 281);
            this.txbxModelText.TabIndex = 2;
            // 
            // tBarResults
            // 
            this.tBarResults.Location = new System.Drawing.Point(12, 315);
            this.tBarResults.Name = "tBarResults";
            this.tBarResults.Size = new System.Drawing.Size(520, 45);
            this.tBarResults.TabIndex = 6;
            this.tBarResults.Scroll += new System.EventHandler(this.tBarResults_Scroll);
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumber.Location = new System.Drawing.Point(551, 324);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(0, 24);
            this.lblNumber.TabIndex = 7;
            // 
            // frmImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 372);
            this.Controls.Add(this.lblNumber);
            this.Controls.Add(this.tBarResults);
            this.Controls.Add(this.txbxModelText);
            this.Controls.Add(this.lblImgPath);
            this.Controls.Add(this.pictureBox1);
            this.Name = "frmImage";
            this.Text = "Red -> Green Prediction Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label lblImgPath;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.TextBox txbxModelText;
        public System.Windows.Forms.TrackBar tBarResults;
        private System.Windows.Forms.Label lblNumber;
    }
}