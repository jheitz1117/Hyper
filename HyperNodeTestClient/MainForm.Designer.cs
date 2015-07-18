using System;
using System.Windows.Forms;

namespace HyperNodeTestClient
{
    partial class MainForm
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
            this.btnToBobViaAlice = new System.Windows.Forms.Button();
            this.lstAliceProgress = new System.Windows.Forms.ListBox();
            this.lblAliceProgress = new System.Windows.Forms.Label();
            this.lblResponse = new System.Windows.Forms.Label();
            this.lstResponse = new System.Windows.Forms.ListBox();
            this.chkReturnTaskTrace = new System.Windows.Forms.CheckBox();
            this.chkRunConcurrently = new System.Windows.Forms.CheckBox();
            this.chkCacheProgressInfo = new System.Windows.Forms.CheckBox();
            this.lblTaskTrace = new System.Windows.Forms.Label();
            this.lblBobProgress = new System.Windows.Forms.Label();
            this.lstBobProgress = new System.Windows.Forms.ListBox();
            this.tvwTaskTrace = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // btnToBobViaAlice
            // 
            this.btnToBobViaAlice.Location = new System.Drawing.Point(12, 12);
            this.btnToBobViaAlice.Name = "btnToBobViaAlice";
            this.btnToBobViaAlice.Size = new System.Drawing.Size(155, 23);
            this.btnToBobViaAlice.TabIndex = 2;
            this.btnToBobViaAlice.Text = "To Bob Via Alice";
            this.btnToBobViaAlice.UseVisualStyleBackColor = true;
            this.btnToBobViaAlice.Click += new System.EventHandler(this.btnToBobViaAlice_Click);
            // 
            // lstAliceProgress
            // 
            this.lstAliceProgress.DisplayMember = "ProgressPercentage";
            this.lstAliceProgress.FormattingEnabled = true;
            this.lstAliceProgress.Location = new System.Drawing.Point(335, 37);
            this.lstAliceProgress.Name = "lstAliceProgress";
            this.lstAliceProgress.Size = new System.Drawing.Size(399, 472);
            this.lstAliceProgress.TabIndex = 3;
            // 
            // lblAliceProgress
            // 
            this.lblAliceProgress.AutoSize = true;
            this.lblAliceProgress.Location = new System.Drawing.Point(332, 22);
            this.lblAliceProgress.Name = "lblAliceProgress";
            this.lblAliceProgress.Size = new System.Drawing.Size(74, 13);
            this.lblAliceProgress.TabIndex = 4;
            this.lblAliceProgress.Text = "Alice Progress";
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Location = new System.Drawing.Point(12, 125);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(55, 13);
            this.lblResponse.TabIndex = 6;
            this.lblResponse.Text = "Response";
            // 
            // lstResponse
            // 
            this.lstResponse.FormattingEnabled = true;
            this.lstResponse.Location = new System.Drawing.Point(12, 141);
            this.lstResponse.Name = "lstResponse";
            this.lstResponse.Size = new System.Drawing.Size(317, 173);
            this.lstResponse.TabIndex = 5;
            // 
            // chkReturnTaskTrace
            // 
            this.chkReturnTaskTrace.AutoSize = true;
            this.chkReturnTaskTrace.Location = new System.Drawing.Point(12, 41);
            this.chkReturnTaskTrace.Name = "chkReturnTaskTrace";
            this.chkReturnTaskTrace.Size = new System.Drawing.Size(116, 17);
            this.chkReturnTaskTrace.TabIndex = 7;
            this.chkReturnTaskTrace.Text = "Return Task Trace";
            this.chkReturnTaskTrace.UseVisualStyleBackColor = true;
            // 
            // chkRunConcurrently
            // 
            this.chkRunConcurrently.AutoSize = true;
            this.chkRunConcurrently.Checked = true;
            this.chkRunConcurrently.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRunConcurrently.Location = new System.Drawing.Point(12, 58);
            this.chkRunConcurrently.Name = "chkRunConcurrently";
            this.chkRunConcurrently.Size = new System.Drawing.Size(135, 17);
            this.chkRunConcurrently.TabIndex = 8;
            this.chkRunConcurrently.Text = "Run Task Concurrently";
            this.chkRunConcurrently.UseVisualStyleBackColor = true;
            // 
            // chkCacheProgressInfo
            // 
            this.chkCacheProgressInfo.AutoSize = true;
            this.chkCacheProgressInfo.Checked = true;
            this.chkCacheProgressInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCacheProgressInfo.Location = new System.Drawing.Point(12, 75);
            this.chkCacheProgressInfo.Name = "chkCacheProgressInfo";
            this.chkCacheProgressInfo.Size = new System.Drawing.Size(122, 17);
            this.chkCacheProgressInfo.TabIndex = 9;
            this.chkCacheProgressInfo.Text = "Cache Progress Info";
            this.chkCacheProgressInfo.UseVisualStyleBackColor = true;
            // 
            // lblTaskTrace
            // 
            this.lblTaskTrace.AutoSize = true;
            this.lblTaskTrace.Location = new System.Drawing.Point(12, 320);
            this.lblTaskTrace.Name = "lblTaskTrace";
            this.lblTaskTrace.Size = new System.Drawing.Size(62, 13);
            this.lblTaskTrace.TabIndex = 11;
            this.lblTaskTrace.Text = "Task Trace";
            // 
            // lblBobProgress
            // 
            this.lblBobProgress.AutoSize = true;
            this.lblBobProgress.Location = new System.Drawing.Point(737, 22);
            this.lblBobProgress.Name = "lblBobProgress";
            this.lblBobProgress.Size = new System.Drawing.Size(70, 13);
            this.lblBobProgress.TabIndex = 13;
            this.lblBobProgress.Text = "Bob Progress";
            // 
            // lstBobProgress
            // 
            this.lstBobProgress.DisplayMember = "ProgressPercentage";
            this.lstBobProgress.FormattingEnabled = true;
            this.lstBobProgress.Location = new System.Drawing.Point(740, 37);
            this.lstBobProgress.Name = "lstBobProgress";
            this.lstBobProgress.Size = new System.Drawing.Size(399, 472);
            this.lstBobProgress.TabIndex = 12;
            // 
            // tvwTaskTrace
            // 
            this.tvwTaskTrace.Location = new System.Drawing.Point(12, 336);
            this.tvwTaskTrace.Name = "tvwTaskTrace";
            this.tvwTaskTrace.Size = new System.Drawing.Size(317, 173);
            this.tvwTaskTrace.TabIndex = 14;
            this.tvwTaskTrace.BeforeExpand += tvwTaskTrace_OnBeforeExpand;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1225, 521);
            this.Controls.Add(this.tvwTaskTrace);
            this.Controls.Add(this.lblBobProgress);
            this.Controls.Add(this.lstBobProgress);
            this.Controls.Add(this.lblTaskTrace);
            this.Controls.Add(this.chkCacheProgressInfo);
            this.Controls.Add(this.chkRunConcurrently);
            this.Controls.Add(this.chkReturnTaskTrace);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.lstResponse);
            this.Controls.Add(this.lblAliceProgress);
            this.Controls.Add(this.lstAliceProgress);
            this.Controls.Add(this.btnToBobViaAlice);
            this.Name = "MainForm";
            this.Text = "HyperNode Test Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.Button btnToBobViaAlice;
        private System.Windows.Forms.ListBox lstAliceProgress;
        private System.Windows.Forms.Label lblAliceProgress;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.ListBox lstResponse;
        private System.Windows.Forms.CheckBox chkReturnTaskTrace;
        private System.Windows.Forms.CheckBox chkRunConcurrently;
        private System.Windows.Forms.CheckBox chkCacheProgressInfo;
        private System.Windows.Forms.Label lblTaskTrace;
        private System.Windows.Forms.Label lblBobProgress;
        private System.Windows.Forms.ListBox lstBobProgress;
        private System.Windows.Forms.TreeView tvwTaskTrace;
    }
}

