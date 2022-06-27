namespace Dashboard
{
    partial class EventSequenceEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventSequenceEditor));
            this.commandEditor = new System.Windows.Forms.RichTextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.edysonFlowControl1 = new Dashboard.EdysonFlowControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.argTab = new System.Windows.Forms.TabPage();
            this.argumentsTextBox = new System.Windows.Forms.TextBox();
            this.chartTab = new System.Windows.Forms.TabPage();
            this.simpleChart1 = new Dashboard.SimpleChart();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.argTab.SuspendLayout();
            this.chartTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandEditor
            // 
            this.commandEditor.Font = new System.Drawing.Font("Cascadia Mono", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.commandEditor.Location = new System.Drawing.Point(0, 0);
            this.commandEditor.Name = "commandEditor";
            this.commandEditor.Size = new System.Drawing.Size(360, 726);
            this.commandEditor.TabIndex = 1;
            this.commandEditor.Text = resources.GetString("commandEditor.Text");
            this.commandEditor.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // listBox1
            // 
            this.listBox1.Enabled = false;
            this.listBox1.Font = new System.Drawing.Font("Cascadia Mono", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Location = new System.Drawing.Point(704, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(439, 676);
            this.listBox1.TabIndex = 2;
            this.listBox1.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.edysonFlowControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 307);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1470, 419);
            this.panel1.TabIndex = 4;
            // 
            // edysonFlowControl1
            // 
            this.edysonFlowControl1.BackColor = System.Drawing.Color.SkyBlue;
            this.edysonFlowControl1.Counter = 0;
            this.edysonFlowControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edysonFlowControl1.Location = new System.Drawing.Point(0, 0);
            this.edysonFlowControl1.Name = "edysonFlowControl1";
            this.edysonFlowControl1.Size = new System.Drawing.Size(1470, 419);
            this.edysonFlowControl1.TabIndex = 0;
            this.edysonFlowControl1.OnSelectObject += new Dashboard.EdysonFlowControl.OnSelectObjectEventHandler(this.edysonFlowControl1_OnSelectObject);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(439, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1031, 307);
            this.panel2.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.argTab);
            this.tabControl1.Controls.Add(this.chartTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1031, 307);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // argTab
            // 
            this.argTab.Controls.Add(this.argumentsTextBox);
            this.argTab.Location = new System.Drawing.Point(4, 29);
            this.argTab.Name = "argTab";
            this.argTab.Padding = new System.Windows.Forms.Padding(3);
            this.argTab.Size = new System.Drawing.Size(1023, 274);
            this.argTab.TabIndex = 0;
            this.argTab.Text = "Arguments";
            this.argTab.UseVisualStyleBackColor = true;
            // 
            // argumentsTextBox
            // 
            this.argumentsTextBox.BackColor = System.Drawing.Color.White;
            this.argumentsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argumentsTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.argumentsTextBox.ForeColor = System.Drawing.Color.MidnightBlue;
            this.argumentsTextBox.Location = new System.Drawing.Point(3, 3);
            this.argumentsTextBox.Multiline = true;
            this.argumentsTextBox.Name = "argumentsTextBox";
            this.argumentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.argumentsTextBox.Size = new System.Drawing.Size(1017, 268);
            this.argumentsTextBox.TabIndex = 0;
            // 
            // chartTab
            // 
            this.chartTab.Controls.Add(this.simpleChart1);
            this.chartTab.Location = new System.Drawing.Point(4, 29);
            this.chartTab.Name = "chartTab";
            this.chartTab.Padding = new System.Windows.Forms.Padding(3);
            this.chartTab.Size = new System.Drawing.Size(1023, 274);
            this.chartTab.TabIndex = 1;
            this.chartTab.Text = "Chart";
            this.chartTab.UseVisualStyleBackColor = true;
            // 
            // simpleChart1
            // 
            this.simpleChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleChart1.Location = new System.Drawing.Point(3, 3);
            this.simpleChart1.Name = "simpleChart1";
            this.simpleChart1.Size = new System.Drawing.Size(1017, 268);
            this.simpleChart1.TabIndex = 0;
            this.simpleChart1.VisibleChanged += new System.EventHandler(this.simpleChart1_VisibleChanged);
            this.simpleChart1.Click += new System.EventHandler(this.simpleChart1_VisibleChanged);
            // 
            // EventSequenceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.commandEditor);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.panel1);
            this.Name = "EventSequenceEditor";
            this.Size = new System.Drawing.Size(1470, 726);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.argTab.ResumeLayout(false);
            this.argTab.PerformLayout();
            this.chartTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private RichTextBox commandEditor;
        private ListBox listBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private Panel panel1;
        private EdysonFlowControl edysonFlowControl1;
        private Panel panel2;
        private TabControl tabControl1;
        private TabPage argTab;
        private TextBox argumentsTextBox;
        private TabPage chartTab;
        private SimpleChart simpleChart1;
    }
}
