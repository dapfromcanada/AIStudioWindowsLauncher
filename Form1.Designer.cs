using ScottPlot.WinForms;

namespace AIStudioWindowsLauncher
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblCpu;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Label lblGpu;
        private System.Windows.Forms.Label lblGpuType;
        private System.Windows.Forms.Label lblDisk;
        private FormsPlot cpuChart;
        private FormsPlot memoryChart;
        private FormsPlot gpuChart;
        private FormsPlot diskChart;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Panel statusPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblCpu = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.lblGpu = new System.Windows.Forms.Label();
            this.lblGpuType = new System.Windows.Forms.Label();
            this.lblDisk = new System.Windows.Forms.Label();
            this.cpuChart = new FormsPlot();
            this.memoryChart = new FormsPlot();
            this.gpuChart = new FormsPlot();
            this.diskChart = new FormsPlot();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.controlPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(10, 10);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 35);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(120, 10);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 35);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(230, 10);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(100, 35);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(10, 10);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(80, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Status: Ready";
            // 
            // lblCpu
            // 
            this.lblCpu.AutoSize = true;
            this.lblCpu.Location = new System.Drawing.Point(350, 15);
            this.lblCpu.Name = "lblCpu";
            this.lblCpu.Size = new System.Drawing.Size(70, 15);
            this.lblCpu.TabIndex = 4;
            this.lblCpu.Text = "CPU: 0.0%";
            // 
            // lblMemory
            // 
            this.lblMemory.AutoSize = true;
            this.lblMemory.Location = new System.Drawing.Point(500, 15);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(100, 15);
            this.lblMemory.TabIndex = 5;
            this.lblMemory.Text = "Memory: 0 MB";
            // 
            // lblGpu
            // 
            this.lblGpu.AutoSize = true;
            this.lblGpu.Location = new System.Drawing.Point(680, 15);
            this.lblGpu.Name = "lblGpu";
            this.lblGpu.Size = new System.Drawing.Size(70, 15);
            this.lblGpu.TabIndex = 6;
            this.lblGpu.Text = "GPU: 0.0%";
            // 
            // lblGpuType
            // 
            this.lblGpuType.AutoSize = true;
            this.lblGpuType.Location = new System.Drawing.Point(350, 30);
            this.lblGpuType.Name = "lblGpuType";
            this.lblGpuType.Size = new System.Drawing.Size(100, 15);
            this.lblGpuType.TabIndex = 7;
            this.lblGpuType.Text = "GPU: Unknown";
            // 
            // lblDisk
            // 
            this.lblDisk.AutoSize = true;
            this.lblDisk.Location = new System.Drawing.Point(500, 30);
            this.lblDisk.Name = "lblDisk";
            this.lblDisk.Size = new System.Drawing.Size(150, 15);
            this.lblDisk.TabIndex = 8;
            this.lblDisk.Text = "Disk: R:0.00 MB/s | W:0.00 MB/s";
            // 
            // cpuChart
            // 
            this.cpuChart.DisplayScale = 1F;
            this.cpuChart.Location = new System.Drawing.Point(10, 130);
            this.cpuChart.Name = "cpuChart";
            this.cpuChart.Size = new System.Drawing.Size(480, 280);
            this.cpuChart.TabIndex = 9;
            // 
            // memoryChart
            // 
            this.memoryChart.DisplayScale = 1F;
            this.memoryChart.Location = new System.Drawing.Point(500, 130);
            this.memoryChart.Name = "memoryChart";
            this.memoryChart.Size = new System.Drawing.Size(480, 280);
            this.memoryChart.TabIndex = 10;
            // 
            // gpuChart
            // 
            this.gpuChart.DisplayScale = 1F;
            this.gpuChart.Location = new System.Drawing.Point(10, 420);
            this.gpuChart.Name = "gpuChart";
            this.gpuChart.Size = new System.Drawing.Size(480, 280);
            this.gpuChart.TabIndex = 11;
            // 
            // diskChart
            // 
            this.diskChart.DisplayScale = 1F;
            this.diskChart.Location = new System.Drawing.Point(500, 420);
            this.diskChart.Name = "diskChart";
            this.diskChart.Size = new System.Drawing.Size(480, 280);
            this.diskChart.TabIndex = 12;
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.btnStart);
            this.controlPanel.Controls.Add(this.btnStop);
            this.controlPanel.Controls.Add(this.btnSettings);
            this.controlPanel.Controls.Add(this.lblCpu);
            this.controlPanel.Controls.Add(this.lblMemory);
            this.controlPanel.Controls.Add(this.lblGpu);
            this.controlPanel.Controls.Add(this.lblGpuType);
            this.controlPanel.Controls.Add(this.lblDisk);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(992, 60);
            this.controlPanel.TabIndex = 13;
            // 
            // statusPanel
            // 
            this.statusPanel.Controls.Add(this.lblStatus);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusPanel.Location = new System.Drawing.Point(0, 60);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(992, 35);
            this.statusPanel.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 715);
            this.Controls.Add(this.diskChart);
            this.Controls.Add(this.gpuChart);
            this.Controls.Add(this.memoryChart);
            this.Controls.Add(this.cpuChart);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.controlPanel);
            this.Name = "Form1";
            this.Text = "AI Studio Windows Launcher";
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
