using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Demo : Form
    {
        public Demo()
        {
            InitializeComponent();
            OpenNewTab("New",
                "publish PulseEvent { \"Name\":\"Pulse\", \"Value\": 120 }\n" +
                "publish PulseEvent { \"Name\":\"Pulse\", \"Value\": 130 }\n" +
                "delay 1000\n" +
                "publish PressureEvent { \"Name\":\"Pressure\", \"Value\": 95 }\n" +
                "publish PressureEvent { \"Name\":\"Pressure\", \"Value\": 110 }\n" +
                "delay 500\n" +
    "delay 2\npublish PulseEvent { \"Name\":\"Pulse\", \"Value\": 110 }\n");

            //OpenNewTab("New", "");
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            if (!runButton.Enabled)
                return;

            runButton.Enabled = false;
            stopButton.Enabled = true;
            //pauseButton.Enabled = true;

            var eventSequenceEditor = GetEventSequenceEditor();
            eventSequenceEditor.Run();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (!stopButton.Enabled)
                return;

            runButton.Enabled = true;
            stopButton.Enabled = false;
            pauseButton.Enabled = false;
            var eventSequenceEditor = GetEventSequenceEditor();
            eventSequenceEditor.Stop();
        }

        private void eventSequenceEditor_OnStop(object sender, EventArgs e)
        {
            runButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Edison scripts|*.edison";
            openDialog.DefaultExt = "edison";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var fInfo = new FileInfo(openDialog.FileName);
                var fileContent = File.ReadAllText(openDialog.FileName);
                OpenNewTab(fInfo.Name, fileContent, fInfo.FullName);
            }
        }

        private void OpenNewTab(string tabName, string text, string? fullFilename = null)
        {
            tabControl.TabPages.Add(tabName);
            var tabPage = tabControl.TabPages[tabControl.TabCount - 1];
            var eventSequenceEditor = new EventSequenceEditor();
            eventSequenceEditor.Value = text;
            eventSequenceEditor.Dock = DockStyle.Fill;
            tabPage.Controls.Add(eventSequenceEditor);
            tabControl.SelectedIndex = tabControl.TabCount - 1;

            if (!string.IsNullOrEmpty(fullFilename))
            {
                var fInfo = new FileInfo(fullFilename);
                if (fInfo.Exists)
                {
                    eventSequenceEditor.Filename = fInfo.FullName;
                    tabPage.Text = fInfo.Name.Replace(".edison", "");
                }
            }

            runButton.Enabled = true;

            eventSequenceEditor.OnChange += new EventHandler(eventSequenceEditorOnChange);
            eventSequenceEditor.OnStop += new EventHandler(eventSequenceEditor_OnStop);
            eventSequenceEditor.Redraw();
        }

        private void saveFileButton_Click(object sender, EventArgs e) => Save();

        private bool SaveAs()
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Edison scripts|*.edison";
            saveDialog.DefaultExt = "edison";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var eventSequenceEditor = GetEventSequenceEditor();
                File.WriteAllText(saveDialog.FileName, eventSequenceEditor.Value);
                eventSequenceEditor.Filename = saveDialog.FileName;
                tabControl.SelectedTab.Text = new FileInfo(saveDialog.FileName).Name.Replace(".edison", "");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Save()
        {
            bool result;
            var eventSequenceEditor = GetEventSequenceEditor();
            if (string.IsNullOrWhiteSpace(eventSequenceEditor.Filename))
            {
                result = SaveAs();
            }
            else
            {
                File.WriteAllText(eventSequenceEditor.Filename, eventSequenceEditor.Value);
                result = true;
            }

            eventSequenceEditor.Saved = result;
            saveFileButton.Enabled = !result;
            return result;
        }

        private void RemoveCurrentTab()
        {
            if (tabControl.SelectedIndex < 0 || tabControl.TabPages.Count == 0)
                return;

            var tabPage = tabControl.TabPages[tabControl.SelectedIndex];
            tabControl.TabPages.Remove(tabPage);

            if (tabControl.TabPages.Count == 0)
            {
                closeFileButton.Enabled = false;
                saveFileButton.Enabled = false;
                runButton.Enabled = false;
            }
        }

        private void closeFileButton_Click(object sender, EventArgs e)
        {
            var eventSequenceEditor = GetEventSequenceEditor();
            if (eventSequenceEditor.Saved)
            {
                RemoveCurrentTab();
            }
            else
            {
                var dialogResult = MessageBox.Show($"File {eventSequenceEditor.Filename} is not saved, do you want to save it?",
                    "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        if (Save()) RemoveCurrentTab();
                        break;
                    case DialogResult.No:
                        RemoveCurrentTab();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
        }

        private EventSequenceEditor GetEventSequenceEditor()
        {
            var tabPage = tabControl.SelectedTab;
            if (tabPage == null || tabPage.Controls.Count == 0)
                return null;

            var eventSequenceEditor = (EventSequenceEditor)tabPage.Controls[0];
            return eventSequenceEditor;
        }

        private void eventSequenceEditor_Load(object sender, EventArgs e)
        {

        }

        private void newFileButton_Click(object sender, EventArgs e)
        {
            OpenNewTab("New", "");
            closeFileButton.Enabled = true;
            saveFileButton.Enabled = false;
        }

        private async void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            await Task.Delay(10);
            var eventSequenceEditor = GetEventSequenceEditor();
            saveFileButton.Enabled = eventSequenceEditor != null && eventSequenceEditor?.Saved != true;

            eventSequenceEditor.Redraw();
        }

        private void eventSequenceEditorOnChange(object sender, EventArgs e)
        {
            saveFileButton.Enabled = true;
        }

        private async void stepOverButton_Click(object sender, EventArgs e)
        {
            if (!stepOverButton.Enabled)
                return;

            runButton.Enabled = false;
            stopButton.Enabled = false;
            pauseButton.Enabled = false;

            await GetEventSequenceEditor().StepOver();
        }
    }
}
