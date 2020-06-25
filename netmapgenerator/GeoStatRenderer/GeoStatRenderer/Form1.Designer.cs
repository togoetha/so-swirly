namespace GeoStatRenderer
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
            this.btnLoadJson = new System.Windows.Forms.Button();
            this.btnFindBounds = new System.Windows.Forms.Button();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.btnDrawBasic = new System.Windows.Forms.Button();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDrawR = new System.Windows.Forms.Button();
            this.btnDrawRGB = new System.Windows.Forms.Button();
            this.btnLoadStat = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOffsetX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOffsetY = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnLoadJson
            // 
            this.btnLoadJson.Location = new System.Drawing.Point(22, 12);
            this.btnLoadJson.Name = "btnLoadJson";
            this.btnLoadJson.Size = new System.Drawing.Size(100, 25);
            this.btnLoadJson.TabIndex = 0;
            this.btnLoadJson.Text = "Load geojson";
            this.btnLoadJson.UseVisualStyleBackColor = true;
            this.btnLoadJson.Click += new System.EventHandler(this.btnLoadJson_Click);
            // 
            // btnFindBounds
            // 
            this.btnFindBounds.Location = new System.Drawing.Point(262, 14);
            this.btnFindBounds.Name = "btnFindBounds";
            this.btnFindBounds.Size = new System.Drawing.Size(105, 23);
            this.btnFindBounds.TabIndex = 1;
            this.btnFindBounds.Text = "Find Bounds";
            this.btnFindBounds.UseVisualStyleBackColor = true;
            this.btnFindBounds.Click += new System.EventHandler(this.btnFindBounds_Click);
            // 
            // pnlMap
            // 
            this.pnlMap.Location = new System.Drawing.Point(12, 55);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(1760, 994);
            this.pnlMap.TabIndex = 2;
            // 
            // btnDrawBasic
            // 
            this.btnDrawBasic.Location = new System.Drawing.Point(753, 14);
            this.btnDrawBasic.Name = "btnDrawBasic";
            this.btnDrawBasic.Size = new System.Drawing.Size(75, 23);
            this.btnDrawBasic.TabIndex = 3;
            this.btnDrawBasic.Text = "Draw!";
            this.btnDrawBasic.UseVisualStyleBackColor = true;
            this.btnDrawBasic.Click += new System.EventHandler(this.btnDrawBasic_Click);
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(436, 15);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(69, 20);
            this.txtScale.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(396, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Scale";
            // 
            // btnDrawR
            // 
            this.btnDrawR.Location = new System.Drawing.Point(834, 14);
            this.btnDrawR.Name = "btnDrawR";
            this.btnDrawR.Size = new System.Drawing.Size(91, 23);
            this.btnDrawR.TabIndex = 6;
            this.btnDrawR.Text = "Draw stat R";
            this.btnDrawR.UseVisualStyleBackColor = true;
            this.btnDrawR.Click += new System.EventHandler(this.btnDrawR_Click);
            // 
            // btnDrawRGB
            // 
            this.btnDrawRGB.Location = new System.Drawing.Point(931, 14);
            this.btnDrawRGB.Name = "btnDrawRGB";
            this.btnDrawRGB.Size = new System.Drawing.Size(94, 23);
            this.btnDrawRGB.TabIndex = 7;
            this.btnDrawRGB.Text = "Draw stat RGB";
            this.btnDrawRGB.UseVisualStyleBackColor = true;
            this.btnDrawRGB.Click += new System.EventHandler(this.btnDrawRGB_Click);
            // 
            // btnLoadStat
            // 
            this.btnLoadStat.Location = new System.Drawing.Point(140, 12);
            this.btnLoadStat.Name = "btnLoadStat";
            this.btnLoadStat.Size = new System.Drawing.Size(96, 25);
            this.btnLoadStat.TabIndex = 8;
            this.btnLoadStat.Text = "Load statistic";
            this.btnLoadStat.UseVisualStyleBackColor = true;
            this.btnLoadStat.Click += new System.EventHandler(this.btnLoadStat_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1031, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(89, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(519, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "OffX";
            // 
            // txtOffsetX
            // 
            this.txtOffsetX.Location = new System.Drawing.Point(559, 15);
            this.txtOffsetX.Name = "txtOffsetX";
            this.txtOffsetX.Size = new System.Drawing.Size(69, 20);
            this.txtOffsetX.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(638, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "OffY";
            // 
            // txtOffsetY
            // 
            this.txtOffsetY.Location = new System.Drawing.Point(678, 15);
            this.txtOffsetY.Name = "txtOffsetY";
            this.txtOffsetY.Size = new System.Drawing.Size(69, 20);
            this.txtOffsetY.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 1061);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOffsetY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOffsetX);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoadStat);
            this.Controls.Add(this.btnDrawRGB);
            this.Controls.Add(this.btnDrawR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtScale);
            this.Controls.Add(this.btnDrawBasic);
            this.Controls.Add(this.pnlMap);
            this.Controls.Add(this.btnFindBounds);
            this.Controls.Add(this.btnLoadJson);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadJson;
        private System.Windows.Forms.Button btnFindBounds;
        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.Button btnDrawBasic;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDrawR;
        private System.Windows.Forms.Button btnDrawRGB;
        private System.Windows.Forms.Button btnLoadStat;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOffsetX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOffsetY;
    }
}

