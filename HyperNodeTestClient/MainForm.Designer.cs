﻿using System;
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
            this.btnRunCommand = new System.Windows.Forms.Button();
            this.lstAliceActivityItems = new System.Windows.Forms.ListBox();
            this.lblRealTimeResponse = new System.Windows.Forms.Label();
            this.lstRealTimeResponse = new System.Windows.Forms.ListBox();
            this.chkReturnTaskTrace = new System.Windows.Forms.CheckBox();
            this.chkRunConcurrently = new System.Windows.Forms.CheckBox();
            this.chkCacheProgressInfo = new System.Windows.Forms.CheckBox();
            this.lblRealTimeTaskTrace = new System.Windows.Forms.Label();
            this.lstBobActivityItems = new System.Windows.Forms.ListBox();
            this.tvwRealTimeTaskTrace = new System.Windows.Forms.TreeView();
            this.lstAliceResponseSummary = new System.Windows.Forms.ListBox();
            this.lstBobResponseSummary = new System.Windows.Forms.ListBox();
            this.cboCommandNames = new System.Windows.Forms.ComboBox();
            this.lblCommandName = new System.Windows.Forms.Label();
            this.grpBobActivityItems = new System.Windows.Forms.GroupBox();
            this.grpBobResponseSummary = new System.Windows.Forms.GroupBox();
            this.spcHyperNodeActivity = new System.Windows.Forms.SplitContainer();
            this.grpAliceActivity = new System.Windows.Forms.GroupBox();
            this.spcAliceActivity = new System.Windows.Forms.SplitContainer();
            this.grpAliceActivityItems = new System.Windows.Forms.GroupBox();
            this.spcAliceResponse = new System.Windows.Forms.SplitContainer();
            this.grpAliceResponseSummary = new System.Windows.Forms.GroupBox();
            this.grpAliceTaskTrace = new System.Windows.Forms.GroupBox();
            this.tvwAliceTaskTrace = new System.Windows.Forms.TreeView();
            this.pnlAliceTop = new System.Windows.Forms.Panel();
            this.txtAliceTaskId = new System.Windows.Forms.TextBox();
            this.btnAliceCancelCurrentTask = new System.Windows.Forms.Button();
            this.lblAliceTaskId = new System.Windows.Forms.Label();
            this.grpBobActivity = new System.Windows.Forms.GroupBox();
            this.spcBobActivity = new System.Windows.Forms.SplitContainer();
            this.spcBobResponse = new System.Windows.Forms.SplitContainer();
            this.grpBobTaskTrace = new System.Windows.Forms.GroupBox();
            this.tvwBobTaskTrace = new System.Windows.Forms.TreeView();
            this.pnlBobTop = new System.Windows.Forms.Panel();
            this.txtBobTaskId = new System.Windows.Forms.TextBox();
            this.btnBobCancelCurrentTask = new System.Windows.Forms.Button();
            this.lblBobTaskId = new System.Windows.Forms.Label();
            this.lblMessageId = new System.Windows.Forms.Label();
            this.txtMessageId = new System.Windows.Forms.TextBox();
            this.pnlHyperNodeActivityTop = new System.Windows.Forms.Panel();
            this.grpHyperNodeActivity = new System.Windows.Forms.GroupBox();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblRecipient = new System.Windows.Forms.Label();
            this.cboHyperNodeNames = new System.Windows.Forms.ComboBox();
            this.btnRefreshCommandList = new System.Windows.Forms.Button();
            this.grpBobActivityItems.SuspendLayout();
            this.grpBobResponseSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcHyperNodeActivity)).BeginInit();
            this.spcHyperNodeActivity.Panel1.SuspendLayout();
            this.spcHyperNodeActivity.Panel2.SuspendLayout();
            this.spcHyperNodeActivity.SuspendLayout();
            this.grpAliceActivity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcAliceActivity)).BeginInit();
            this.spcAliceActivity.Panel1.SuspendLayout();
            this.spcAliceActivity.Panel2.SuspendLayout();
            this.spcAliceActivity.SuspendLayout();
            this.grpAliceActivityItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcAliceResponse)).BeginInit();
            this.spcAliceResponse.Panel1.SuspendLayout();
            this.spcAliceResponse.Panel2.SuspendLayout();
            this.spcAliceResponse.SuspendLayout();
            this.grpAliceResponseSummary.SuspendLayout();
            this.grpAliceTaskTrace.SuspendLayout();
            this.pnlAliceTop.SuspendLayout();
            this.grpBobActivity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcBobActivity)).BeginInit();
            this.spcBobActivity.Panel1.SuspendLayout();
            this.spcBobActivity.Panel2.SuspendLayout();
            this.spcBobActivity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcBobResponse)).BeginInit();
            this.spcBobResponse.Panel1.SuspendLayout();
            this.spcBobResponse.Panel2.SuspendLayout();
            this.spcBobResponse.SuspendLayout();
            this.grpBobTaskTrace.SuspendLayout();
            this.pnlBobTop.SuspendLayout();
            this.pnlHyperNodeActivityTop.SuspendLayout();
            this.grpHyperNodeActivity.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRunCommand
            // 
            this.btnRunCommand.Location = new System.Drawing.Point(291, 61);
            this.btnRunCommand.Name = "btnRunCommand";
            this.btnRunCommand.Size = new System.Drawing.Size(75, 23);
            this.btnRunCommand.TabIndex = 2;
            this.btnRunCommand.Text = "Run";
            this.btnRunCommand.UseVisualStyleBackColor = true;
            this.btnRunCommand.Click += new System.EventHandler(this.btnRunCommand_Click);
            // 
            // lstAliceActivityItems
            // 
            this.lstAliceActivityItems.DisplayMember = "ProgressPercentage";
            this.lstAliceActivityItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAliceActivityItems.FormattingEnabled = true;
            this.lstAliceActivityItems.Location = new System.Drawing.Point(3, 16);
            this.lstAliceActivityItems.Name = "lstAliceActivityItems";
            this.lstAliceActivityItems.Size = new System.Drawing.Size(567, 234);
            this.lstAliceActivityItems.TabIndex = 3;
            // 
            // lblRealTimeResponse
            // 
            this.lblRealTimeResponse.AutoSize = true;
            this.lblRealTimeResponse.Location = new System.Drawing.Point(19, 333);
            this.lblRealTimeResponse.Name = "lblRealTimeResponse";
            this.lblRealTimeResponse.Size = new System.Drawing.Size(106, 13);
            this.lblRealTimeResponse.TabIndex = 6;
            this.lblRealTimeResponse.Text = "Real Time Response";
            // 
            // lstRealTimeResponse
            // 
            this.lstRealTimeResponse.FormattingEnabled = true;
            this.lstRealTimeResponse.Location = new System.Drawing.Point(19, 349);
            this.lstRealTimeResponse.Name = "lstRealTimeResponse";
            this.lstRealTimeResponse.Size = new System.Drawing.Size(317, 173);
            this.lstRealTimeResponse.TabIndex = 5;
            // 
            // chkReturnTaskTrace
            // 
            this.chkReturnTaskTrace.AutoSize = true;
            this.chkReturnTaskTrace.Location = new System.Drawing.Point(66, 90);
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
            this.chkRunConcurrently.Location = new System.Drawing.Point(66, 107);
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
            this.chkCacheProgressInfo.Location = new System.Drawing.Point(66, 124);
            this.chkCacheProgressInfo.Name = "chkCacheProgressInfo";
            this.chkCacheProgressInfo.Size = new System.Drawing.Size(122, 17);
            this.chkCacheProgressInfo.TabIndex = 9;
            this.chkCacheProgressInfo.Text = "Cache Progress Info";
            this.chkCacheProgressInfo.UseVisualStyleBackColor = true;
            // 
            // lblRealTimeTaskTrace
            // 
            this.lblRealTimeTaskTrace.AutoSize = true;
            this.lblRealTimeTaskTrace.Location = new System.Drawing.Point(19, 528);
            this.lblRealTimeTaskTrace.Name = "lblRealTimeTaskTrace";
            this.lblRealTimeTaskTrace.Size = new System.Drawing.Size(113, 13);
            this.lblRealTimeTaskTrace.TabIndex = 11;
            this.lblRealTimeTaskTrace.Text = "Real Time Task Trace";
            // 
            // lstBobActivityItems
            // 
            this.lstBobActivityItems.DisplayMember = "ProgressPercentage";
            this.lstBobActivityItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBobActivityItems.FormattingEnabled = true;
            this.lstBobActivityItems.Location = new System.Drawing.Point(3, 16);
            this.lstBobActivityItems.Name = "lstBobActivityItems";
            this.lstBobActivityItems.Size = new System.Drawing.Size(562, 234);
            this.lstBobActivityItems.TabIndex = 12;
            // 
            // tvwRealTimeTaskTrace
            // 
            this.tvwRealTimeTaskTrace.Location = new System.Drawing.Point(19, 544);
            this.tvwRealTimeTaskTrace.Name = "tvwRealTimeTaskTrace";
            this.tvwRealTimeTaskTrace.Size = new System.Drawing.Size(317, 118);
            this.tvwRealTimeTaskTrace.TabIndex = 14;
            // 
            // lstAliceResponseSummary
            // 
            this.lstAliceResponseSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAliceResponseSummary.FormattingEnabled = true;
            this.lstAliceResponseSummary.Location = new System.Drawing.Point(3, 16);
            this.lstAliceResponseSummary.Name = "lstAliceResponseSummary";
            this.lstAliceResponseSummary.Size = new System.Drawing.Size(336, 316);
            this.lstAliceResponseSummary.TabIndex = 15;
            // 
            // lstBobResponseSummary
            // 
            this.lstBobResponseSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBobResponseSummary.FormattingEnabled = true;
            this.lstBobResponseSummary.Location = new System.Drawing.Point(3, 16);
            this.lstBobResponseSummary.Name = "lstBobResponseSummary";
            this.lstBobResponseSummary.Size = new System.Drawing.Size(331, 316);
            this.lstBobResponseSummary.TabIndex = 17;
            // 
            // cboCommandNames
            // 
            this.cboCommandNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCommandNames.FormattingEnabled = true;
            this.cboCommandNames.Location = new System.Drawing.Point(66, 33);
            this.cboCommandNames.Name = "cboCommandNames";
            this.cboCommandNames.Size = new System.Drawing.Size(219, 21);
            this.cboCommandNames.TabIndex = 20;
            // 
            // lblCommandName
            // 
            this.lblCommandName.AutoSize = true;
            this.lblCommandName.Location = new System.Drawing.Point(6, 36);
            this.lblCommandName.Name = "lblCommandName";
            this.lblCommandName.Size = new System.Drawing.Size(54, 13);
            this.lblCommandName.TabIndex = 21;
            this.lblCommandName.Text = "Command";
            // 
            // grpBobActivityItems
            // 
            this.grpBobActivityItems.Controls.Add(this.lstBobActivityItems);
            this.grpBobActivityItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBobActivityItems.Location = new System.Drawing.Point(0, 0);
            this.grpBobActivityItems.Name = "grpBobActivityItems";
            this.grpBobActivityItems.Size = new System.Drawing.Size(568, 253);
            this.grpBobActivityItems.TabIndex = 19;
            this.grpBobActivityItems.TabStop = false;
            this.grpBobActivityItems.Text = "Activity Items";
            // 
            // grpBobResponseSummary
            // 
            this.grpBobResponseSummary.Controls.Add(this.lstBobResponseSummary);
            this.grpBobResponseSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBobResponseSummary.Location = new System.Drawing.Point(0, 0);
            this.grpBobResponseSummary.Name = "grpBobResponseSummary";
            this.grpBobResponseSummary.Size = new System.Drawing.Size(337, 335);
            this.grpBobResponseSummary.TabIndex = 20;
            this.grpBobResponseSummary.TabStop = false;
            this.grpBobResponseSummary.Text = "Response Summary";
            // 
            // spcHyperNodeActivity
            // 
            this.spcHyperNodeActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcHyperNodeActivity.Location = new System.Drawing.Point(3, 42);
            this.spcHyperNodeActivity.Name = "spcHyperNodeActivity";
            // 
            // spcHyperNodeActivity.Panel1
            // 
            this.spcHyperNodeActivity.Panel1.Controls.Add(this.grpAliceActivity);
            // 
            // spcHyperNodeActivity.Panel2
            // 
            this.spcHyperNodeActivity.Panel2.Controls.Add(this.grpBobActivity);
            this.spcHyperNodeActivity.Size = new System.Drawing.Size(1157, 637);
            this.spcHyperNodeActivity.SplitterDistance = 579;
            this.spcHyperNodeActivity.TabIndex = 23;
            // 
            // grpAliceActivity
            // 
            this.grpAliceActivity.Controls.Add(this.spcAliceActivity);
            this.grpAliceActivity.Controls.Add(this.pnlAliceTop);
            this.grpAliceActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAliceActivity.Location = new System.Drawing.Point(0, 0);
            this.grpAliceActivity.Name = "grpAliceActivity";
            this.grpAliceActivity.Size = new System.Drawing.Size(579, 637);
            this.grpAliceActivity.TabIndex = 5;
            this.grpAliceActivity.TabStop = false;
            this.grpAliceActivity.Text = "Alice Activity";
            // 
            // spcAliceActivity
            // 
            this.spcAliceActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcAliceActivity.Location = new System.Drawing.Point(3, 42);
            this.spcAliceActivity.Name = "spcAliceActivity";
            this.spcAliceActivity.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcAliceActivity.Panel1
            // 
            this.spcAliceActivity.Panel1.Controls.Add(this.grpAliceActivityItems);
            // 
            // spcAliceActivity.Panel2
            // 
            this.spcAliceActivity.Panel2.Controls.Add(this.spcAliceResponse);
            this.spcAliceActivity.Size = new System.Drawing.Size(573, 592);
            this.spcAliceActivity.SplitterDistance = 253;
            this.spcAliceActivity.TabIndex = 28;
            // 
            // grpAliceActivityItems
            // 
            this.grpAliceActivityItems.Controls.Add(this.lstAliceActivityItems);
            this.grpAliceActivityItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAliceActivityItems.Location = new System.Drawing.Point(0, 0);
            this.grpAliceActivityItems.Name = "grpAliceActivityItems";
            this.grpAliceActivityItems.Size = new System.Drawing.Size(573, 253);
            this.grpAliceActivityItems.TabIndex = 25;
            this.grpAliceActivityItems.TabStop = false;
            this.grpAliceActivityItems.Text = "Activity Items";
            // 
            // spcAliceResponse
            // 
            this.spcAliceResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcAliceResponse.Location = new System.Drawing.Point(0, 0);
            this.spcAliceResponse.Name = "spcAliceResponse";
            // 
            // spcAliceResponse.Panel1
            // 
            this.spcAliceResponse.Panel1.Controls.Add(this.grpAliceResponseSummary);
            // 
            // spcAliceResponse.Panel2
            // 
            this.spcAliceResponse.Panel2.Controls.Add(this.grpAliceTaskTrace);
            this.spcAliceResponse.Size = new System.Drawing.Size(573, 335);
            this.spcAliceResponse.SplitterDistance = 342;
            this.spcAliceResponse.TabIndex = 0;
            // 
            // grpAliceResponseSummary
            // 
            this.grpAliceResponseSummary.Controls.Add(this.lstAliceResponseSummary);
            this.grpAliceResponseSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAliceResponseSummary.Location = new System.Drawing.Point(0, 0);
            this.grpAliceResponseSummary.Name = "grpAliceResponseSummary";
            this.grpAliceResponseSummary.Size = new System.Drawing.Size(342, 335);
            this.grpAliceResponseSummary.TabIndex = 26;
            this.grpAliceResponseSummary.TabStop = false;
            this.grpAliceResponseSummary.Text = "Response Summary";
            // 
            // grpAliceTaskTrace
            // 
            this.grpAliceTaskTrace.Controls.Add(this.tvwAliceTaskTrace);
            this.grpAliceTaskTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAliceTaskTrace.Location = new System.Drawing.Point(0, 0);
            this.grpAliceTaskTrace.Name = "grpAliceTaskTrace";
            this.grpAliceTaskTrace.Size = new System.Drawing.Size(227, 335);
            this.grpAliceTaskTrace.TabIndex = 27;
            this.grpAliceTaskTrace.TabStop = false;
            this.grpAliceTaskTrace.Text = "Task Trace";
            // 
            // tvwAliceTaskTrace
            // 
            this.tvwAliceTaskTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwAliceTaskTrace.Location = new System.Drawing.Point(3, 16);
            this.tvwAliceTaskTrace.Name = "tvwAliceTaskTrace";
            this.tvwAliceTaskTrace.Size = new System.Drawing.Size(221, 316);
            this.tvwAliceTaskTrace.TabIndex = 15;
            // 
            // pnlAliceTop
            // 
            this.pnlAliceTop.Controls.Add(this.txtAliceTaskId);
            this.pnlAliceTop.Controls.Add(this.btnAliceCancelCurrentTask);
            this.pnlAliceTop.Controls.Add(this.lblAliceTaskId);
            this.pnlAliceTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAliceTop.Location = new System.Drawing.Point(3, 16);
            this.pnlAliceTop.Name = "pnlAliceTop";
            this.pnlAliceTop.Padding = new System.Windows.Forms.Padding(3);
            this.pnlAliceTop.Size = new System.Drawing.Size(573, 26);
            this.pnlAliceTop.TabIndex = 4;
            // 
            // txtAliceTaskId
            // 
            this.txtAliceTaskId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliceTaskId.Location = new System.Drawing.Point(51, 3);
            this.txtAliceTaskId.Name = "txtAliceTaskId";
            this.txtAliceTaskId.ReadOnly = true;
            this.txtAliceTaskId.Size = new System.Drawing.Size(444, 20);
            this.txtAliceTaskId.TabIndex = 1;
            // 
            // btnAliceCancelCurrentTask
            // 
            this.btnAliceCancelCurrentTask.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAliceCancelCurrentTask.Location = new System.Drawing.Point(495, 3);
            this.btnAliceCancelCurrentTask.Name = "btnAliceCancelCurrentTask";
            this.btnAliceCancelCurrentTask.Size = new System.Drawing.Size(75, 20);
            this.btnAliceCancelCurrentTask.TabIndex = 2;
            this.btnAliceCancelCurrentTask.Text = "Cancel";
            this.btnAliceCancelCurrentTask.UseVisualStyleBackColor = true;
            this.btnAliceCancelCurrentTask.Click += new System.EventHandler(this.btnAliceCancelCurrentTask_Click);
            // 
            // lblAliceTaskId
            // 
            this.lblAliceTaskId.AutoSize = true;
            this.lblAliceTaskId.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblAliceTaskId.Location = new System.Drawing.Point(3, 3);
            this.lblAliceTaskId.Name = "lblAliceTaskId";
            this.lblAliceTaskId.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.lblAliceTaskId.Size = new System.Drawing.Size(48, 16);
            this.lblAliceTaskId.TabIndex = 0;
            this.lblAliceTaskId.Text = "Task ID";
            // 
            // grpBobActivity
            // 
            this.grpBobActivity.Controls.Add(this.spcBobActivity);
            this.grpBobActivity.Controls.Add(this.pnlBobTop);
            this.grpBobActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBobActivity.Location = new System.Drawing.Point(0, 0);
            this.grpBobActivity.Name = "grpBobActivity";
            this.grpBobActivity.Size = new System.Drawing.Size(574, 637);
            this.grpBobActivity.TabIndex = 7;
            this.grpBobActivity.TabStop = false;
            this.grpBobActivity.Text = "Bob Activity";
            // 
            // spcBobActivity
            // 
            this.spcBobActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcBobActivity.Location = new System.Drawing.Point(3, 42);
            this.spcBobActivity.Name = "spcBobActivity";
            this.spcBobActivity.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcBobActivity.Panel1
            // 
            this.spcBobActivity.Panel1.Controls.Add(this.grpBobActivityItems);
            // 
            // spcBobActivity.Panel2
            // 
            this.spcBobActivity.Panel2.Controls.Add(this.spcBobResponse);
            this.spcBobActivity.Size = new System.Drawing.Size(568, 592);
            this.spcBobActivity.SplitterDistance = 253;
            this.spcBobActivity.TabIndex = 29;
            // 
            // spcBobResponse
            // 
            this.spcBobResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcBobResponse.Location = new System.Drawing.Point(0, 0);
            this.spcBobResponse.Name = "spcBobResponse";
            // 
            // spcBobResponse.Panel1
            // 
            this.spcBobResponse.Panel1.Controls.Add(this.grpBobResponseSummary);
            // 
            // spcBobResponse.Panel2
            // 
            this.spcBobResponse.Panel2.Controls.Add(this.grpBobTaskTrace);
            this.spcBobResponse.Size = new System.Drawing.Size(568, 335);
            this.spcBobResponse.SplitterDistance = 337;
            this.spcBobResponse.TabIndex = 0;
            // 
            // grpBobTaskTrace
            // 
            this.grpBobTaskTrace.Controls.Add(this.tvwBobTaskTrace);
            this.grpBobTaskTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBobTaskTrace.Location = new System.Drawing.Point(0, 0);
            this.grpBobTaskTrace.Name = "grpBobTaskTrace";
            this.grpBobTaskTrace.Size = new System.Drawing.Size(227, 335);
            this.grpBobTaskTrace.TabIndex = 27;
            this.grpBobTaskTrace.TabStop = false;
            this.grpBobTaskTrace.Text = "Task Trace";
            // 
            // tvwBobTaskTrace
            // 
            this.tvwBobTaskTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwBobTaskTrace.Location = new System.Drawing.Point(3, 16);
            this.tvwBobTaskTrace.Name = "tvwBobTaskTrace";
            this.tvwBobTaskTrace.Size = new System.Drawing.Size(221, 316);
            this.tvwBobTaskTrace.TabIndex = 15;
            // 
            // pnlBobTop
            // 
            this.pnlBobTop.Controls.Add(this.txtBobTaskId);
            this.pnlBobTop.Controls.Add(this.btnBobCancelCurrentTask);
            this.pnlBobTop.Controls.Add(this.lblBobTaskId);
            this.pnlBobTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBobTop.Location = new System.Drawing.Point(3, 16);
            this.pnlBobTop.Name = "pnlBobTop";
            this.pnlBobTop.Padding = new System.Windows.Forms.Padding(3);
            this.pnlBobTop.Size = new System.Drawing.Size(568, 26);
            this.pnlBobTop.TabIndex = 6;
            // 
            // txtBobTaskId
            // 
            this.txtBobTaskId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBobTaskId.Location = new System.Drawing.Point(51, 3);
            this.txtBobTaskId.Name = "txtBobTaskId";
            this.txtBobTaskId.ReadOnly = true;
            this.txtBobTaskId.Size = new System.Drawing.Size(439, 20);
            this.txtBobTaskId.TabIndex = 1;
            // 
            // btnBobCancelCurrentTask
            // 
            this.btnBobCancelCurrentTask.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBobCancelCurrentTask.Location = new System.Drawing.Point(490, 3);
            this.btnBobCancelCurrentTask.Name = "btnBobCancelCurrentTask";
            this.btnBobCancelCurrentTask.Size = new System.Drawing.Size(75, 20);
            this.btnBobCancelCurrentTask.TabIndex = 3;
            this.btnBobCancelCurrentTask.Text = "Cancel";
            this.btnBobCancelCurrentTask.UseVisualStyleBackColor = true;
            this.btnBobCancelCurrentTask.Click += new System.EventHandler(this.btnBobCancelCurrentTask_Click);
            // 
            // lblBobTaskId
            // 
            this.lblBobTaskId.AutoSize = true;
            this.lblBobTaskId.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblBobTaskId.Location = new System.Drawing.Point(3, 3);
            this.lblBobTaskId.Name = "lblBobTaskId";
            this.lblBobTaskId.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.lblBobTaskId.Size = new System.Drawing.Size(48, 16);
            this.lblBobTaskId.TabIndex = 0;
            this.lblBobTaskId.Text = "Task ID";
            // 
            // lblMessageId
            // 
            this.lblMessageId.AutoSize = true;
            this.lblMessageId.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblMessageId.Location = new System.Drawing.Point(3, 3);
            this.lblMessageId.Name = "lblMessageId";
            this.lblMessageId.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.lblMessageId.Size = new System.Drawing.Size(67, 16);
            this.lblMessageId.TabIndex = 0;
            this.lblMessageId.Text = "Message ID";
            // 
            // txtMessageId
            // 
            this.txtMessageId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageId.Location = new System.Drawing.Point(70, 3);
            this.txtMessageId.Name = "txtMessageId";
            this.txtMessageId.ReadOnly = true;
            this.txtMessageId.Size = new System.Drawing.Size(1084, 20);
            this.txtMessageId.TabIndex = 1;
            // 
            // pnlHyperNodeActivityTop
            // 
            this.pnlHyperNodeActivityTop.Controls.Add(this.txtMessageId);
            this.pnlHyperNodeActivityTop.Controls.Add(this.lblMessageId);
            this.pnlHyperNodeActivityTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHyperNodeActivityTop.Location = new System.Drawing.Point(3, 16);
            this.pnlHyperNodeActivityTop.Name = "pnlHyperNodeActivityTop";
            this.pnlHyperNodeActivityTop.Padding = new System.Windows.Forms.Padding(3);
            this.pnlHyperNodeActivityTop.Size = new System.Drawing.Size(1157, 26);
            this.pnlHyperNodeActivityTop.TabIndex = 0;
            // 
            // grpHyperNodeActivity
            // 
            this.grpHyperNodeActivity.Controls.Add(this.spcHyperNodeActivity);
            this.grpHyperNodeActivity.Controls.Add(this.pnlHyperNodeActivityTop);
            this.grpHyperNodeActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHyperNodeActivity.Location = new System.Drawing.Point(369, 0);
            this.grpHyperNodeActivity.Name = "grpHyperNodeActivity";
            this.grpHyperNodeActivity.Size = new System.Drawing.Size(1163, 682);
            this.grpHyperNodeActivity.TabIndex = 24;
            this.grpHyperNodeActivity.TabStop = false;
            this.grpHyperNodeActivity.Text = "HyperNode Activity";
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.lblRecipient);
            this.pnlLeft.Controls.Add(this.cboHyperNodeNames);
            this.pnlLeft.Controls.Add(this.btnRefreshCommandList);
            this.pnlLeft.Controls.Add(this.btnRunCommand);
            this.pnlLeft.Controls.Add(this.lstRealTimeResponse);
            this.pnlLeft.Controls.Add(this.lblCommandName);
            this.pnlLeft.Controls.Add(this.lblRealTimeResponse);
            this.pnlLeft.Controls.Add(this.cboCommandNames);
            this.pnlLeft.Controls.Add(this.chkReturnTaskTrace);
            this.pnlLeft.Controls.Add(this.chkRunConcurrently);
            this.pnlLeft.Controls.Add(this.tvwRealTimeTaskTrace);
            this.pnlLeft.Controls.Add(this.chkCacheProgressInfo);
            this.pnlLeft.Controls.Add(this.lblRealTimeTaskTrace);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(369, 682);
            this.pnlLeft.TabIndex = 25;
            // 
            // lblRecipient
            // 
            this.lblRecipient.AutoSize = true;
            this.lblRecipient.Location = new System.Drawing.Point(6, 7);
            this.lblRecipient.Name = "lblRecipient";
            this.lblRecipient.Size = new System.Drawing.Size(52, 13);
            this.lblRecipient.TabIndex = 24;
            this.lblRecipient.Text = "Recipient";
            // 
            // cboHyperNodeNames
            // 
            this.cboHyperNodeNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHyperNodeNames.FormattingEnabled = true;
            this.cboHyperNodeNames.Location = new System.Drawing.Point(66, 4);
            this.cboHyperNodeNames.Name = "cboHyperNodeNames";
            this.cboHyperNodeNames.Size = new System.Drawing.Size(219, 21);
            this.cboHyperNodeNames.TabIndex = 23;
            // 
            // btnRefreshCommandList
            // 
            this.btnRefreshCommandList.Location = new System.Drawing.Point(291, 32);
            this.btnRefreshCommandList.Name = "btnRefreshCommandList";
            this.btnRefreshCommandList.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshCommandList.TabIndex = 22;
            this.btnRefreshCommandList.Text = "Refresh";
            this.btnRefreshCommandList.UseVisualStyleBackColor = true;
            this.btnRefreshCommandList.Click += new System.EventHandler(this.btnRefreshCommandList_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1532, 682);
            this.Controls.Add(this.grpHyperNodeActivity);
            this.Controls.Add(this.pnlLeft);
            this.Name = "MainForm";
            this.Text = "HyperNode Test Client";
            this.grpBobActivityItems.ResumeLayout(false);
            this.grpBobResponseSummary.ResumeLayout(false);
            this.spcHyperNodeActivity.Panel1.ResumeLayout(false);
            this.spcHyperNodeActivity.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcHyperNodeActivity)).EndInit();
            this.spcHyperNodeActivity.ResumeLayout(false);
            this.grpAliceActivity.ResumeLayout(false);
            this.spcAliceActivity.Panel1.ResumeLayout(false);
            this.spcAliceActivity.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcAliceActivity)).EndInit();
            this.spcAliceActivity.ResumeLayout(false);
            this.grpAliceActivityItems.ResumeLayout(false);
            this.spcAliceResponse.Panel1.ResumeLayout(false);
            this.spcAliceResponse.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcAliceResponse)).EndInit();
            this.spcAliceResponse.ResumeLayout(false);
            this.grpAliceResponseSummary.ResumeLayout(false);
            this.grpAliceTaskTrace.ResumeLayout(false);
            this.pnlAliceTop.ResumeLayout(false);
            this.pnlAliceTop.PerformLayout();
            this.grpBobActivity.ResumeLayout(false);
            this.spcBobActivity.Panel1.ResumeLayout(false);
            this.spcBobActivity.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcBobActivity)).EndInit();
            this.spcBobActivity.ResumeLayout(false);
            this.spcBobResponse.Panel1.ResumeLayout(false);
            this.spcBobResponse.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcBobResponse)).EndInit();
            this.spcBobResponse.ResumeLayout(false);
            this.grpBobTaskTrace.ResumeLayout(false);
            this.pnlBobTop.ResumeLayout(false);
            this.pnlBobTop.PerformLayout();
            this.pnlHyperNodeActivityTop.ResumeLayout(false);
            this.pnlHyperNodeActivityTop.PerformLayout();
            this.grpHyperNodeActivity.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.ResumeLayout(false);

        }
        
        #endregion

        private System.Windows.Forms.Button btnRunCommand;
        private System.Windows.Forms.ListBox lstAliceActivityItems;
        private System.Windows.Forms.Label lblRealTimeResponse;
        private System.Windows.Forms.ListBox lstRealTimeResponse;
        private System.Windows.Forms.CheckBox chkReturnTaskTrace;
        private System.Windows.Forms.CheckBox chkRunConcurrently;
        private System.Windows.Forms.CheckBox chkCacheProgressInfo;
        private System.Windows.Forms.Label lblRealTimeTaskTrace;
        private System.Windows.Forms.ListBox lstBobActivityItems;
        private System.Windows.Forms.TreeView tvwRealTimeTaskTrace;
        private ListBox lstAliceResponseSummary;
        private ListBox lstBobResponseSummary;
        private ComboBox cboCommandNames;
        private Label lblCommandName;
        private GroupBox grpBobResponseSummary;
        private GroupBox grpBobActivityItems;
        private SplitContainer spcHyperNodeActivity;
        private Panel pnlAliceTop;
        private TextBox txtAliceTaskId;
        private Label lblAliceTaskId;
        private Panel pnlBobTop;
        private TextBox txtBobTaskId;
        private Label lblBobTaskId;
        private Label lblMessageId;
        private TextBox txtMessageId;
        private Panel pnlHyperNodeActivityTop;
        private GroupBox grpHyperNodeActivity;
        private GroupBox grpAliceActivity;
        private GroupBox grpBobActivity;
        private GroupBox grpAliceActivityItems;
        private GroupBox grpAliceResponseSummary;
        private GroupBox grpAliceTaskTrace;
        private TreeView tvwAliceTaskTrace;
        private SplitContainer spcAliceActivity;
        private SplitContainer spcAliceResponse;
        private SplitContainer spcBobActivity;
        private SplitContainer spcBobResponse;
        private GroupBox grpBobTaskTrace;
        private TreeView tvwBobTaskTrace;
        private Panel pnlLeft;
        private Button btnRefreshCommandList;
        private Label lblRecipient;
        private ComboBox cboHyperNodeNames;
        private Button btnAliceCancelCurrentTask;
        private Button btnBobCancelCurrentTask;
    }
}

