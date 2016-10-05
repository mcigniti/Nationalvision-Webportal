namespace Automation.Mercury.Engine
{
    partial class form_SuiteRunner
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
            this.btn_LoadTests = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lstbox_Module = new System.Windows.Forms.ListBox();
            this.lstbox_Criteria = new System.Windows.Forms.ListBox();
            this.lbl_Modules = new System.Windows.Forms.Label();
            this.lbl_Criteria = new System.Windows.Forms.Label();
            this.btn_FilterData = new System.Windows.Forms.Button();
            this.btn_ExecuteTests = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.lbl_OverallExecutionStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstbox_SubModule = new System.Windows.Forms.ListBox();
            this.lnklbl_OverallExeStatResult = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_LoadTests
            // 
            this.btn_LoadTests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_LoadTests.Location = new System.Drawing.Point(105, 169);
            this.btn_LoadTests.Name = "btn_LoadTests";
            this.btn_LoadTests.Size = new System.Drawing.Size(75, 46);
            this.btn_LoadTests.TabIndex = 0;
            this.btn_LoadTests.Text = "LoadTests";
            this.btn_LoadTests.UseVisualStyleBackColor = true;
            this.btn_LoadTests.Click += new System.EventHandler(this.btn_LoadTests_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 246);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1133, 426);
            this.dataGridView1.TabIndex = 1;
            // 
            // lstbox_Module
            // 
            this.lstbox_Module.FormattingEnabled = true;
            this.lstbox_Module.Location = new System.Drawing.Point(219, 145);
            this.lstbox_Module.Name = "lstbox_Module";
            this.lstbox_Module.Size = new System.Drawing.Size(254, 95);
            this.lstbox_Module.TabIndex = 2;
            // 
            // lstbox_Criteria
            // 
            this.lstbox_Criteria.FormattingEnabled = true;
            this.lstbox_Criteria.Location = new System.Drawing.Point(842, 145);
            this.lstbox_Criteria.Name = "lstbox_Criteria";
            this.lstbox_Criteria.Size = new System.Drawing.Size(210, 95);
            this.lstbox_Criteria.TabIndex = 3;
            // 
            // lbl_Modules
            // 
            this.lbl_Modules.AutoSize = true;
            this.lbl_Modules.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Modules.Location = new System.Drawing.Point(216, 111);
            this.lbl_Modules.Name = "lbl_Modules";
            this.lbl_Modules.Size = new System.Drawing.Size(97, 13);
            this.lbl_Modules.TabIndex = 5;
            this.lbl_Modules.Text = "Filter by Module";
            // 
            // lbl_Criteria
            // 
            this.lbl_Criteria.AutoSize = true;
            this.lbl_Criteria.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Criteria.Location = new System.Drawing.Point(839, 111);
            this.lbl_Criteria.Name = "lbl_Criteria";
            this.lbl_Criteria.Size = new System.Drawing.Size(106, 13);
            this.lbl_Criteria.TabIndex = 6;
            this.lbl_Criteria.Text = "Filter by Category";
            // 
            // btn_FilterData
            // 
            this.btn_FilterData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_FilterData.Location = new System.Drawing.Point(1070, 145);
            this.btn_FilterData.Name = "btn_FilterData";
            this.btn_FilterData.Size = new System.Drawing.Size(75, 95);
            this.btn_FilterData.TabIndex = 7;
            this.btn_FilterData.Text = "FilterData";
            this.btn_FilterData.UseVisualStyleBackColor = true;
            this.btn_FilterData.Click += new System.EventHandler(this.btn_FilterData_Click);
            // 
            // btn_ExecuteTests
            // 
            this.btn_ExecuteTests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ExecuteTests.Location = new System.Drawing.Point(1151, 343);
            this.btn_ExecuteTests.Name = "btn_ExecuteTests";
            this.btn_ExecuteTests.Size = new System.Drawing.Size(111, 95);
            this.btn_ExecuteTests.TabIndex = 8;
            this.btn_ExecuteTests.Text = "ExecuteTests";
            this.btn_ExecuteTests.UseVisualStyleBackColor = true;
            this.btn_ExecuteTests.Click += new System.EventHandler(this.btn_ExecuteTests_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectAll.Location = new System.Drawing.Point(1151, 221);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(111, 95);
            this.btnSelectAll.TabIndex = 9;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // lbl_OverallExecutionStatus
            // 
            this.lbl_OverallExecutionStatus.AutoSize = true;
            this.lbl_OverallExecutionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_OverallExecutionStatus.Location = new System.Drawing.Point(1053, 111);
            this.lbl_OverallExecutionStatus.Name = "lbl_OverallExecutionStatus";
            this.lbl_OverallExecutionStatus.Size = new System.Drawing.Size(143, 13);
            this.lbl_OverallExecutionStatus.TabIndex = 10;
            this.lbl_OverallExecutionStatus.Text = "OverallExecutionStatus:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(504, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Filter by SubModule";
            // 
            // lstbox_SubModule
            // 
            this.lstbox_SubModule.FormattingEnabled = true;
            this.lstbox_SubModule.Location = new System.Drawing.Point(507, 145);
            this.lstbox_SubModule.Name = "lstbox_SubModule";
            this.lstbox_SubModule.Size = new System.Drawing.Size(299, 95);
            this.lstbox_SubModule.TabIndex = 12;
            // 
            // lnklbl_OverallExeStatResult
            // 
            this.lnklbl_OverallExeStatResult.AutoSize = true;
            this.lnklbl_OverallExeStatResult.Location = new System.Drawing.Point(1202, 111);
            this.lnklbl_OverallExeStatResult.Name = "lnklbl_OverallExeStatResult";
            this.lnklbl_OverallExeStatResult.Size = new System.Drawing.Size(59, 13);
            this.lnklbl_OverallExeStatResult.TabIndex = 14;
            this.lnklbl_OverallExeStatResult.TabStop = true;
            this.lnklbl_OverallExeStatResult.Text = "Not started";
            this.lnklbl_OverallExeStatResult.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklbl_OverallExeStatResult_LinkClicked);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(1100, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(151, 59);
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(4, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(162, 144);
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(4, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(162, 144);
            this.pictureBox3.TabIndex = 17;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // form_SuiteRunner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 509);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lnklbl_OverallExeStatResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstbox_SubModule);
            this.Controls.Add(this.lbl_OverallExecutionStatus);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btn_ExecuteTests);
            this.Controls.Add(this.btn_FilterData);
            this.Controls.Add(this.lbl_Criteria);
            this.Controls.Add(this.lbl_Modules);
            this.Controls.Add(this.lstbox_Criteria);
            this.Controls.Add(this.lstbox_Module);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btn_LoadTests);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_SuiteRunner";
            this.Text = "Suite Runner";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.form_SuiteRunner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_LoadTests;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ListBox lstbox_Module;
        private System.Windows.Forms.ListBox lstbox_Criteria;
        private System.Windows.Forms.Label lbl_Modules;
        private System.Windows.Forms.Label lbl_Criteria;
        private System.Windows.Forms.Button btn_FilterData;
        private System.Windows.Forms.Button btn_ExecuteTests;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Label lbl_OverallExecutionStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstbox_SubModule;
        private System.Windows.Forms.LinkLabel lnklbl_OverallExeStatResult;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}

