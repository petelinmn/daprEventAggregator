namespace Dashboard
{
    partial class ConfigurationEditor
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
            this.eaTextBox = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.StereotypesTextBox = new System.Windows.Forms.RichTextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.newPersonNameTextBox = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarBPHigh = new System.Windows.Forms.TrackBar();
            this.bloodPressureLabel = new System.Windows.Forms.Label();
            this.trackBarBPLow = new System.Windows.Forms.TrackBar();
            this.trackBarPulse = new System.Windows.Forms.TrackBar();
            this.labelPulseValue = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBPHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBPLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPulse)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // eaTextBox
            // 
            this.eaTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eaTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.eaTextBox.Location = new System.Drawing.Point(3, 3);
            this.eaTextBox.Name = "eaTextBox";
            this.eaTextBox.Size = new System.Drawing.Size(767, 1016);
            this.eaTextBox.TabIndex = 1;
            this.eaTextBox.Text = "";
            this.eaTextBox.TextChanged += new System.EventHandler(this.configurationEditor2_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl1.Location = new System.Drawing.Point(705, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(781, 1055);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.eaTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(773, 1022);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "EventAggregator";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.StereotypesTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(773, 1022);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Stereotypes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // StereotypesTextBox
            // 
            this.StereotypesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StereotypesTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StereotypesTextBox.Location = new System.Drawing.Point(3, 3);
            this.StereotypesTextBox.Name = "StereotypesTextBox";
            this.StereotypesTextBox.Size = new System.Drawing.Size(767, 1016);
            this.StereotypesTextBox.TabIndex = 2;
            this.StereotypesTextBox.Text = "";
            this.StereotypesTextBox.TextChanged += new System.EventHandler(this.StereotypesTextBox_TextChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(12, 29);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(574, 84);
            this.listBox1.TabIndex = 3;
            // 
            // newPersonNameTextBox
            // 
            this.newPersonNameTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.newPersonNameTextBox.Location = new System.Drawing.Point(12, 8);
            this.newPersonNameTextBox.Name = "newPersonNameTextBox";
            this.newPersonNameTextBox.Size = new System.Drawing.Size(244, 38);
            this.newPersonNameTextBox.TabIndex = 4;
            this.newPersonNameTextBox.TextChanged += new System.EventHandler(this.newPersonNameTextBox_TextChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dateTimePicker1.Location = new System.Drawing.Point(262, 8);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(350, 34);
            this.dateTimePicker1.TabIndex = 5;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 28);
            this.label1.TabIndex = 6;
            this.label1.Text = "Cardiovascular System";
            // 
            // trackBarBPHigh
            // 
            this.trackBarBPHigh.Location = new System.Drawing.Point(261, 153);
            this.trackBarBPHigh.Maximum = 190;
            this.trackBarBPHigh.Minimum = 90;
            this.trackBarBPHigh.Name = "trackBarBPHigh";
            this.trackBarBPHigh.Size = new System.Drawing.Size(259, 56);
            this.trackBarBPHigh.TabIndex = 7;
            this.trackBarBPHigh.Value = 120;
            this.trackBarBPHigh.Scroll += new System.EventHandler(this.trackBarBPLow_Scroll);
            // 
            // bloodPressureLabel
            // 
            this.bloodPressureLabel.AutoSize = true;
            this.bloodPressureLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bloodPressureLabel.Location = new System.Drawing.Point(12, 122);
            this.bloodPressureLabel.Name = "bloodPressureLabel";
            this.bloodPressureLabel.Size = new System.Drawing.Size(141, 28);
            this.bloodPressureLabel.TabIndex = 9;
            this.bloodPressureLabel.Text = "Blood Pressure";
            // 
            // trackBarBPLow
            // 
            this.trackBarBPLow.Location = new System.Drawing.Point(12, 153);
            this.trackBarBPLow.Maximum = 120;
            this.trackBarBPLow.Minimum = 40;
            this.trackBarBPLow.Name = "trackBarBPLow";
            this.trackBarBPLow.Size = new System.Drawing.Size(243, 56);
            this.trackBarBPLow.TabIndex = 10;
            this.trackBarBPLow.Value = 110;
            this.trackBarBPLow.Scroll += new System.EventHandler(this.trackBarBPLow_Scroll);
            // 
            // trackBarPulse
            // 
            this.trackBarPulse.Location = new System.Drawing.Point(14, 215);
            this.trackBarPulse.Maximum = 160;
            this.trackBarPulse.Minimum = 60;
            this.trackBarPulse.Name = "trackBarPulse";
            this.trackBarPulse.Size = new System.Drawing.Size(256, 56);
            this.trackBarPulse.TabIndex = 11;
            this.trackBarPulse.Value = 160;
            this.trackBarPulse.Scroll += new System.EventHandler(this.trackBarPulse_Scroll);
            // 
            // labelPulseValue
            // 
            this.labelPulseValue.AutoSize = true;
            this.labelPulseValue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelPulseValue.Location = new System.Drawing.Point(304, 215);
            this.labelPulseValue.Name = "labelPulseValue";
            this.labelPulseValue.Size = new System.Drawing.Size(61, 28);
            this.labelPulseValue.TabIndex = 12;
            this.labelPulseValue.Text = "Pulse:";
            this.labelPulseValue.Click += new System.EventHandler(this.labelPulseValue_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.labelPulseValue);
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.trackBarPulse);
            this.panel1.Controls.Add(this.trackBarBPHigh);
            this.panel1.Controls.Add(this.trackBarBPLow);
            this.panel1.Controls.Add(this.bloodPressureLabel);
            this.panel1.Location = new System.Drawing.Point(12, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 265);
            this.panel1.TabIndex = 13;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(526, 206);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(60, 40);
            this.button6.TabIndex = 15;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(526, 144);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(60, 40);
            this.button5.TabIndex = 14;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(628, 1055);
            this.panel2.TabIndex = 14;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(628, 1055);
            this.splitContainer1.SplitterDistance = 522;
            this.splitContainer1.TabIndex = 17;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listBox3);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.dateTimePicker1);
            this.panel3.Controls.Add(this.button3);
            this.panel3.Controls.Add(this.newPersonNameTextBox);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(628, 522);
            this.panel3.TabIndex = 16;
            // 
            // listBox3
            // 
            this.listBox3.FormattingEnabled = true;
            this.listBox3.ItemHeight = 20;
            this.listBox3.Location = new System.Drawing.Point(429, 361);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(183, 144);
            this.listBox3.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(350, 361);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 28);
            this.label3.TabIndex = 17;
            this.label3.Text = "Actions:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(131, 361);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(217, 148);
            this.textBox1.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 358);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 28);
            this.label2.TabIndex = 15;
            this.label2.Text = "Stereotypes:";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button3.Location = new System.Drawing.Point(316, 48);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(296, 36);
            this.button3.TabIndex = 14;
            this.button3.Text = "New";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(12, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(298, 36);
            this.button2.TabIndex = 13;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(628, 529);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 31;
            this.listBox2.Location = new System.Drawing.Point(628, 0);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(77, 1055);
            this.listBox2.TabIndex = 15;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ConfigurationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1486, 1055);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tabControl1);
            this.Name = "ConfigurationEditor";
            this.Text = "ConfigurationEditor";
            this.Load += new System.EventHandler(this.ConfigurationEditor_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBPHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBPLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPulse)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private RichTextBox eaTextBox;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private RichTextBox StereotypesTextBox;
        private ListBox listBox1;
        private TextBox newPersonNameTextBox;
        private DateTimePicker dateTimePicker1;
        private Label label1;
        private TrackBar trackBarBPHigh;
        private Label bloodPressureLabel;
        private TrackBar trackBarBPLow;
        private TrackBar trackBarPulse;
        private Label labelPulseValue;
        private Panel panel1;
        private Panel panel2;
        private ListBox listBox2;
        private Button button2;
        private System.Windows.Forms.Timer timer1;
        private Button button3;
        private RichTextBox richTextBox1;
        private Panel panel3;
        private SplitContainer splitContainer1;
        private Label label2;
        private TextBox textBox1;
        private Button button6;
        private Button button5;
        private ListBox listBox3;
        private Label label3;
    }
}