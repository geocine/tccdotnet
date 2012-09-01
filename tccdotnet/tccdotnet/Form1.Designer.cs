namespace tccdotnet
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            AIMS.Libraries.CodeEditor.WinForms.LineMarginRender lineMarginRender1 = new AIMS.Libraries.CodeEditor.WinForms.LineMarginRender();
            this.syntaxDocument = new AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusError = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelOutput = new System.Windows.Forms.Panel();
            this.tabOutput = new tccdotnet.Controls.TabControlEx();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.headerMessage = new tccdotnet.Controls.HeaderLabel();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.codeEditorControl1 = new AIMS.Libraries.CodeEditor.CodeEditorControl();
            this.tvParseTree = new System.Windows.Forms.TreeView();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelOutput.SuspendLayout();
            this.tabOutput.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // syntaxDocument
            // 
            this.syntaxDocument.Lines = new string[] {
        ""};
            this.syntaxDocument.MaxUndoBufferSize = 1000;
            this.syntaxDocument.Modified = false;
            this.syntaxDocument.UndoStep = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(679, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Check";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Open";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "Show Messages";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusError});
            this.statusStrip.Location = new System.Drawing.Point(0, 418);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(679, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusError
            // 
            this.statusError.Name = "statusError";
            this.statusError.Size = new System.Drawing.Size(39, 17);
            this.statusError.Text = "Ready";
            // 
            // panelOutput
            // 
            this.panelOutput.Controls.Add(this.tabOutput);
            this.panelOutput.Controls.Add(this.headerMessage);
            this.panelOutput.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelOutput.Location = new System.Drawing.Point(479, 25);
            this.panelOutput.Name = "panelOutput";
            this.panelOutput.Size = new System.Drawing.Size(200, 393);
            this.panelOutput.TabIndex = 5;
            // 
            // tabOutput
            // 
            this.tabOutput.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabOutput.Controls.Add(this.tabPage1);
            this.tabOutput.Controls.Add(this.tabPage2);
            this.tabOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabOutput.Location = new System.Drawing.Point(0, 21);
            this.tabOutput.Name = "tabOutput";
            this.tabOutput.SelectedIndex = 0;
            this.tabOutput.Size = new System.Drawing.Size(200, 372);
            this.tabOutput.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tvParseTree);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 346);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Parse Tree";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 346);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // headerMessage
            // 
            this.headerMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerMessage.ForeColor = System.Drawing.SystemColors.GrayText;
            this.headerMessage.Location = new System.Drawing.Point(0, 0);
            this.headerMessage.Name = "headerMessage";
            this.headerMessage.Size = new System.Drawing.Size(200, 21);
            this.headerMessage.TabIndex = 1;
            this.headerMessage.Text = "Messages";
            this.headerMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.headerMessage.UseCompatibleTextRendering = true;
            // 
            // splitterRight
            // 
            this.splitterRight.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(474, 25);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(5, 393);
            this.splitterRight.TabIndex = 8;
            this.splitterRight.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.codeEditorControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(474, 393);
            this.panel1.TabIndex = 9;
            // 
            // codeEditorControl1
            // 
            this.codeEditorControl1.ActiveView = AIMS.Libraries.CodeEditor.WinForms.ActiveView.BottomRight;
            this.codeEditorControl1.AllowBreakPoints = false;
            this.codeEditorControl1.AutoListPosition = null;
            this.codeEditorControl1.AutoListSelectedText = "";
            this.codeEditorControl1.AutoListVisible = false;
            this.codeEditorControl1.CopyAsRTF = false;
            this.codeEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditorControl1.Document = this.syntaxDocument;
            this.codeEditorControl1.FileName = null;
            this.codeEditorControl1.InfoTipCount = 1;
            this.codeEditorControl1.InfoTipPosition = null;
            this.codeEditorControl1.InfoTipSelectedIndex = 1;
            this.codeEditorControl1.InfoTipVisible = false;
            lineMarginRender1.Bounds = new System.Drawing.Rectangle(19, 0, 19, 16);
            this.codeEditorControl1.LineMarginRender = lineMarginRender1;
            this.codeEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.codeEditorControl1.LockCursorUpdate = false;
            this.codeEditorControl1.Name = "codeEditorControl1";
            this.codeEditorControl1.Saved = false;
            this.codeEditorControl1.ShowScopeIndicator = false;
            this.codeEditorControl1.Size = new System.Drawing.Size(474, 393);
            this.codeEditorControl1.SmoothScroll = false;
            this.codeEditorControl1.SplitView = false;
            this.codeEditorControl1.SplitviewH = -4;
            this.codeEditorControl1.SplitviewV = -4;
            this.codeEditorControl1.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.codeEditorControl1.TabIndex = 1;
            this.codeEditorControl1.Tag = "";
            this.codeEditorControl1.Text = "codeEditor";
            this.codeEditorControl1.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            // 
            // tvParseTree
            // 
            this.tvParseTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvParseTree.Location = new System.Drawing.Point(3, 3);
            this.tvParseTree.Name = "tvParseTree";
            this.tvParseTree.Size = new System.Drawing.Size(186, 340);
            this.tvParseTree.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 440);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.panelOutput);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Tag = "cc";
            this.Text = "tccdotnet : A C Compiler created in .NET";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelOutput.ResumeLayout(false);
            this.tabOutput.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument syntaxDocument;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusError;
        private System.Windows.Forms.Panel panelOutput;
        private System.Windows.Forms.Splitter splitterRight;
        private System.Windows.Forms.Panel panel1;
        private AIMS.Libraries.CodeEditor.CodeEditorControl codeEditorControl1;
        private tccdotnet.Controls.HeaderLabel headerMessage;
        private tccdotnet.Controls.TabControlEx tabOutput;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.TreeView tvParseTree;
    }
}

