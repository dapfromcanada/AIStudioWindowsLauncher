using System;
using System.Windows.Forms;
using System.IO;

namespace AIStudioWindowsLauncher
{
    public partial class SettingsForm : Form
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string PythonAppPath { get; set; }
        
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int SamplingRateMs { get; set; }

        public SettingsForm(string currentPath, int currentSamplingRate)
        {
            InitializeComponent();
            PythonAppPath = currentPath;
            SamplingRateMs = currentSamplingRate;
            
            txtPath.Text = PythonAppPath;
            
            // Populate sampling rate combo box
            cmbSamplingRate.Items.AddRange(new object[] { "0.5s", "1s", "2s", "5s", "10s", "30s", "60s" });
            
            // Set current selection based on milliseconds
            string currentRate = (currentSamplingRate / 1000.0) + "s";
            int index = cmbSamplingRate.FindString(currentRate);
            cmbSamplingRate.SelectedIndex = index >= 0 ? index : 1; // Default to 1s
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = PythonAppPath;
                dialog.Description = "Select AI Studio folder location";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtPath.Text))
            {
                MessageBox.Show("The specified path does not exist.", "Invalid Path", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PythonAppPath = txtPath.Text;
            
            // Parse sampling rate from selection
            string selectedRate = cmbSamplingRate.SelectedItem.ToString();
            double seconds = double.Parse(selectedRate.TrimEnd('s'));
            SamplingRateMs = (int)(seconds * 1000);
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
