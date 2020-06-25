namespace ImageTransform
{
    partial class Form2
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtEdgenodes = new System.Windows.Forms.TextBox();
            this.txtFognodes = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoadNodes = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPingDistDiff = new System.Windows.Forms.TextBox();
            this.pnlCluster = new System.Windows.Forms.Panel();
            this.btnSolveDist = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMinPing = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSLAMaxPing = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMaxPing = new System.Windows.Forms.TextBox();
            this.btnIncrSolve = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtRndServiceInstances = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSimRounds = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtJoinsRound = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLeavesRound = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkRf = new System.Windows.Forms.CheckBox();
            this.chkRp = new System.Windows.Forms.CheckBox();
            this.btnLoadDensity = new System.Windows.Forms.Button();
            this.btnSolveRandom = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtAvgMedPing = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtClusters = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtFogCapacity = new System.Windows.Forms.TextBox();
            this.chkRe = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(199, 16);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(94, 23);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generate nodes";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtEdgenodes
            // 
            this.txtEdgenodes.Location = new System.Drawing.Point(111, 18);
            this.txtEdgenodes.Name = "txtEdgenodes";
            this.txtEdgenodes.Size = new System.Drawing.Size(74, 20);
            this.txtEdgenodes.TabIndex = 3;
            // 
            // txtFognodes
            // 
            this.txtFognodes.Location = new System.Drawing.Point(111, 44);
            this.txtFognodes.Name = "txtFognodes";
            this.txtFognodes.Size = new System.Drawing.Size(74, 20);
            this.txtFognodes.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(199, 43);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save nodes";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnLoadNodes
            // 
            this.btnLoadNodes.Location = new System.Drawing.Point(199, 68);
            this.btnLoadNodes.Name = "btnLoadNodes";
            this.btnLoadNodes.Size = new System.Drawing.Size(94, 23);
            this.btnLoadNodes.TabIndex = 7;
            this.btnLoadNodes.Text = "Load nodes";
            this.btnLoadNodes.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Edge nodes:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Fog nodes:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Max ping/dist diff (%)";
            // 
            // txtPingDistDiff
            // 
            this.txtPingDistDiff.Location = new System.Drawing.Point(123, 71);
            this.txtPingDistDiff.Name = "txtPingDistDiff";
            this.txtPingDistDiff.Size = new System.Drawing.Size(63, 20);
            this.txtPingDistDiff.TabIndex = 11;
            // 
            // pnlCluster
            // 
            this.pnlCluster.Location = new System.Drawing.Point(12, 114);
            this.pnlCluster.Name = "pnlCluster";
            this.pnlCluster.Size = new System.Drawing.Size(1200, 750);
            this.pnlCluster.TabIndex = 12;
            // 
            // btnSolveDist
            // 
            this.btnSolveDist.Location = new System.Drawing.Point(146, 14);
            this.btnSolveDist.Name = "btnSolveDist";
            this.btnSolveDist.Size = new System.Drawing.Size(119, 23);
            this.btnSolveDist.TabIndex = 14;
            this.btnSolveDist.Text = "!Solve distance";
            this.btnSolveDist.UseVisualStyleBackColor = true;
            this.btnSolveDist.Click += new System.EventHandler(this.btnSolveDist_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Min ping:";
            // 
            // txtMinPing
            // 
            this.txtMinPing.Location = new System.Drawing.Point(65, 39);
            this.txtMinPing.Name = "txtMinPing";
            this.txtMinPing.Size = new System.Drawing.Size(61, 20);
            this.txtMinPing.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "SLA max ping:";
            // 
            // txtSLAMaxPing
            // 
            this.txtSLAMaxPing.Location = new System.Drawing.Point(87, 17);
            this.txtSLAMaxPing.Name = "txtSLAMaxPing";
            this.txtSLAMaxPing.Size = new System.Drawing.Size(75, 20);
            this.txtSLAMaxPing.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Max ping:";
            // 
            // txtMaxPing
            // 
            this.txtMaxPing.Location = new System.Drawing.Point(65, 14);
            this.txtMaxPing.Name = "txtMaxPing";
            this.txtMaxPing.Size = new System.Drawing.Size(61, 20);
            this.txtMaxPing.TabIndex = 19;
            // 
            // btnIncrSolve
            // 
            this.btnIncrSolve.Location = new System.Drawing.Point(146, 41);
            this.btnIncrSolve.Name = "btnIncrSolve";
            this.btnIncrSolve.Size = new System.Drawing.Size(119, 23);
            this.btnIncrSolve.TabIndex = 21;
            this.btnIncrSolve.Text = "Incremental solve";
            this.btnIncrSolve.UseVisualStyleBackColor = true;
            this.btnIncrSolve.Click += new System.EventHandler(this.btnIncrSolve_click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnLoadNodes);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.txtFognodes);
            this.groupBox1.Controls.Add(this.txtEdgenodes);
            this.groupBox1.Controls.Add(this.btnGenerate);
            this.groupBox1.Controls.Add(this.txtPingDistDiff);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(303, 99);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Node generation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.txtFogCapacity);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtRndServiceInstances);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtSimRounds);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtJoinsRound);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtLeavesRound);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtSLAMaxPing);
            this.groupBox2.Location = new System.Drawing.Point(321, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(326, 98);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parameters";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(168, 48);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Instances:";
            // 
            // txtRndServiceInstances
            // 
            this.txtRndServiceInstances.Location = new System.Drawing.Point(230, 44);
            this.txtRndServiceInstances.Name = "txtRndServiceInstances";
            this.txtRndServiceInstances.Size = new System.Drawing.Size(84, 20);
            this.txtRndServiceInstances.TabIndex = 25;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(168, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Rounds:";
            // 
            // txtSimRounds
            // 
            this.txtSimRounds.Location = new System.Drawing.Point(230, 17);
            this.txtSimRounds.Name = "txtSimRounds";
            this.txtSimRounds.Size = new System.Drawing.Size(84, 20);
            this.txtSimRounds.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Joins/round:";
            // 
            // txtJoinsRound
            // 
            this.txtJoinsRound.Location = new System.Drawing.Point(87, 69);
            this.txtJoinsRound.Name = "txtJoinsRound";
            this.txtJoinsRound.Size = new System.Drawing.Size(75, 20);
            this.txtJoinsRound.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Leaves/round:";
            // 
            // txtLeavesRound
            // 
            this.txtLeavesRound.Location = new System.Drawing.Point(87, 44);
            this.txtLeavesRound.Name = "txtLeavesRound";
            this.txtLeavesRound.Size = new System.Drawing.Size(75, 20);
            this.txtLeavesRound.TabIndex = 19;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkRe);
            this.groupBox3.Controls.Add(this.chkRf);
            this.groupBox3.Controls.Add(this.chkRp);
            this.groupBox3.Controls.Add(this.btnLoadDensity);
            this.groupBox3.Controls.Add(this.btnSolveRandom);
            this.groupBox3.Controls.Add(this.btnIncrSolve);
            this.groupBox3.Controls.Add(this.btnSolveDist);
            this.groupBox3.Location = new System.Drawing.Point(653, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(285, 98);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Solvers";
            // 
            // chkRf
            // 
            this.chkRf.AutoSize = true;
            this.chkRf.Location = new System.Drawing.Point(21, 58);
            this.chkRf.Name = "chkRf";
            this.chkRf.Size = new System.Drawing.Size(63, 17);
            this.chkRf.TabIndex = 26;
            this.chkRf.Text = "draw Rf";
            this.chkRf.UseVisualStyleBackColor = true;
            // 
            // chkRp
            // 
            this.chkRp.AutoSize = true;
            this.chkRp.Location = new System.Drawing.Point(21, 38);
            this.chkRp.Name = "chkRp";
            this.chkRp.Size = new System.Drawing.Size(66, 17);
            this.chkRp.TabIndex = 25;
            this.chkRp.Text = "draw Rp";
            this.chkRp.UseVisualStyleBackColor = true;
            // 
            // btnLoadDensity
            // 
            this.btnLoadDensity.Location = new System.Drawing.Point(21, 14);
            this.btnLoadDensity.Name = "btnLoadDensity";
            this.btnLoadDensity.Size = new System.Drawing.Size(119, 23);
            this.btnLoadDensity.TabIndex = 24;
            this.btnLoadDensity.Text = "Load density";
            this.btnLoadDensity.UseVisualStyleBackColor = true;
            this.btnLoadDensity.Click += new System.EventHandler(this.btnLoadDensity_Click);
            // 
            // btnSolveRandom
            // 
            this.btnSolveRandom.Location = new System.Drawing.Point(146, 68);
            this.btnSolveRandom.Name = "btnSolveRandom";
            this.btnSolveRandom.Size = new System.Drawing.Size(119, 23);
            this.btnSolveRandom.TabIndex = 23;
            this.btnSolveRandom.Text = "Solve random (k8s)";
            this.btnSolveRandom.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Avg/median ping:";
            // 
            // txtAvgMedPing
            // 
            this.txtAvgMedPing.Location = new System.Drawing.Point(103, 65);
            this.txtAvgMedPing.Name = "txtAvgMedPing";
            this.txtAvgMedPing.Size = new System.Drawing.Size(70, 20);
            this.txtAvgMedPing.TabIndex = 26;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.txtTime);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.txtClusters);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.txtAvgMedPing);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.txtMaxPing);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txtMinPing);
            this.groupBox4.Location = new System.Drawing.Point(944, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(265, 98);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Results";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(132, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 13);
            this.label12.TabIndex = 31;
            this.label12.Text = "Time:";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(191, 15);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(58, 20);
            this.txtTime.TabIndex = 30;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(132, 43);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 13);
            this.label13.TabIndex = 29;
            this.label13.Text = "Clusters:";
            // 
            // txtClusters
            // 
            this.txtClusters.Location = new System.Drawing.Point(191, 40);
            this.txtClusters.Name = "txtClusters";
            this.txtClusters.Size = new System.Drawing.Size(58, 20);
            this.txtClusters.TabIndex = 28;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(168, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 13);
            this.label14.TabIndex = 28;
            this.label14.Text = "Fnode cap:";
            // 
            // txtFogCapacity
            // 
            this.txtFogCapacity.Location = new System.Drawing.Point(230, 70);
            this.txtFogCapacity.Name = "txtFogCapacity";
            this.txtFogCapacity.Size = new System.Drawing.Size(84, 20);
            this.txtFogCapacity.TabIndex = 27;
            // 
            // chkRe
            // 
            this.chkRe.AutoSize = true;
            this.chkRe.Location = new System.Drawing.Point(21, 81);
            this.chkRe.Name = "chkRe";
            this.chkRe.Size = new System.Drawing.Size(66, 17);
            this.chkRe.TabIndex = 27;
            this.chkRe.Text = "draw Re";
            this.chkRe.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 876);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlCluster);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtEdgenodes;
        private System.Windows.Forms.TextBox txtFognodes;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoadNodes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPingDistDiff;
        private System.Windows.Forms.Panel pnlCluster;
        private System.Windows.Forms.Button btnSolveDist;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMinPing;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSLAMaxPing;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMaxPing;
        private System.Windows.Forms.Button btnIncrSolve;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSimRounds;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtJoinsRound;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLeavesRound;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtAvgMedPing;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtRndServiceInstances;
        private System.Windows.Forms.Button btnSolveRandom;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtClusters;
        private System.Windows.Forms.Button btnLoadDensity;
        private System.Windows.Forms.CheckBox chkRf;
        private System.Windows.Forms.CheckBox chkRp;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtFogCapacity;
        private System.Windows.Forms.CheckBox chkRe;
    }
}