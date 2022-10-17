namespace TSI_DL_cognexIntegration
{
    partial class frmMain
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
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.pictureBoxShow = new System.Windows.Forms.PictureBox();
            this.txbxImagePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFinalPred = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cxbxResize = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShow)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadImage.Location = new System.Drawing.Point(25, 12);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(186, 37);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "Load Image";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // pictureBoxShow
            // 
            this.pictureBoxShow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxShow.Location = new System.Drawing.Point(2, 8);
            this.pictureBoxShow.Name = "pictureBoxShow";
            this.pictureBoxShow.Size = new System.Drawing.Size(1658, 433);
            this.pictureBoxShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxShow.TabIndex = 1;
            this.pictureBoxShow.TabStop = false;
            // 
            // txbxImagePath
            // 
            this.txbxImagePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbxImagePath.Location = new System.Drawing.Point(347, 18);
            this.txbxImagePath.Name = "txbxImagePath";
            this.txbxImagePath.Size = new System.Drawing.Size(988, 24);
            this.txbxImagePath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(217, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Path of Image";
            // 
            // btnFinalPred
            // 
            this.btnFinalPred.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinalPred.Location = new System.Drawing.Point(27, 55);
            this.btnFinalPred.Name = "btnFinalPred";
            this.btnFinalPred.Size = new System.Drawing.Size(184, 72);
            this.btnFinalPred.TabIndex = 13;
            this.btnFinalPred.Text = "make production prediction";
            this.btnFinalPred.UseVisualStyleBackColor = true;
            this.btnFinalPred.Click += new System.EventHandler(this.btnFinalPred_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBoxShow);
            this.panel1.Location = new System.Drawing.Point(25, 184);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1664, 448);
            this.panel1.TabIndex = 15;
            // 
            // cxbxResize
            // 
            this.cxbxResize.AutoSize = true;
            this.cxbxResize.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cxbxResize.Location = new System.Drawing.Point(48, 133);
            this.cxbxResize.Name = "cxbxResize";
            this.cxbxResize.Size = new System.Drawing.Size(146, 28);
            this.cxbxResize.TabIndex = 16;
            this.cxbxResize.Text = "resize image?";
            this.cxbxResize.UseVisualStyleBackColor = true;
            this.cxbxResize.CheckedChanged += new System.EventHandler(this.cxbxResize_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1701, 649);
            this.Controls.Add(this.cxbxResize);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnFinalPred);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbxImagePath);
            this.Controls.Add(this.btnLoadImage);
            this.Name = "frmMain";
            this.Text = "TSI - Testing Cognex Deep Vision Pro Library";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShow)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txbxImagePath;
        public System.Windows.Forms.PictureBox pictureBoxShow;
        public System.Windows.Forms.Button btnFinalPred;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.CheckBox cxbxResize;
    }
}

