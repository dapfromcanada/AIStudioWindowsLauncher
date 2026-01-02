using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ScottPlot;
using ScottPlot.WinForms;

namespace AIStudioWindowsLauncher
{
    public partial class Form1 : Form
    {
        private SystemMonitor monitor;
        private MetricsLogger logger;
        private System.Windows.Forms.Timer monitoringTimer;
        private Process pythonProcess;
        
        private string aiStudioPath = @"G:\AIStudio";
        private int samplingRateMs = 1000; // Default 1 second
        
        private bool isMonitoring = false;
        private List<SystemMetrics> metricsHistory = new List<SystemMetrics>();
        private const int MAX_HISTORY_POINTS = 300; // 5 minutes at 1 sec sampling

        public Form1()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            monitor = new SystemMonitor();
            monitoringTimer = new System.Windows.Forms.Timer();
            monitoringTimer.Tick += MonitoringTimer_Tick;
            
            // Display GPU type
            lblGpuType.Text = $"GPU: {monitor.GpuType}";
            
            // Update status
            UpdateStatus("Ready");
            
            // Initialize charts
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            // CPU Chart
            cpuChart.Plot.Title("CPU Usage (%)");
            cpuChart.Plot.XLabel("Time");
            cpuChart.Plot.YLabel("%");
            cpuChart.Plot.Axes.SetLimitsY(0, 100);
            
            // Memory Chart
            memoryChart.Plot.Title("Memory Usage (%)");
            memoryChart.Plot.XLabel("Time");
            memoryChart.Plot.YLabel("%");
            memoryChart.Plot.Axes.SetLimitsY(0, 100);
            
            // GPU Chart
            gpuChart.Plot.Title("GPU Usage (%)");
            gpuChart.Plot.XLabel("Time");
            gpuChart.Plot.YLabel("%");
            gpuChart.Plot.Axes.SetLimitsY(0, 100);
            
            // Disk I/O Chart
            diskChart.Plot.Title("Disk I/O (MB/s)");
            diskChart.Plot.XLabel("Time");
            diskChart.Plot.YLabel("MB/s");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(aiStudioPath))
            {
                MessageBox.Show($"AI Studio path not found: {aiStudioPath}\n\nPlease configure the path in Settings.",
                    "Path Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartMonitoring();
            LaunchPythonApp();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMonitoring();
            StopPythonApp();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(aiStudioPath, samplingRateMs))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    aiStudioPath = settingsForm.PythonAppPath;
                    samplingRateMs = settingsForm.SamplingRateMs;
                    
                    // Update timer interval if monitoring
                    if (isMonitoring)
                    {
                        monitoringTimer.Interval = samplingRateMs;
                    }
                }
            }
        }

        private void LaunchPythonApp()
        {
            try
            {
                // Look for common Python entry points
                string[] possibleFiles = { "main.py", "app.py", "run.py", "__main__.py" };
                string pythonScript = null;
                
                foreach (var file in possibleFiles)
                {
                    string fullPath = Path.Combine(aiStudioPath, file);
                    if (File.Exists(fullPath))
                    {
                        pythonScript = fullPath;
                        break;
                    }
                }

                if (pythonScript == null)
                {
                    UpdateStatus("No Python entry point found (main.py, app.py, run.py)");
                    return;
                }

                // Check for virtual environment
                string pythonExe = "python";
                string venvPath = Path.Combine(aiStudioPath, "venv", "Scripts", "python.exe");
                if (File.Exists(venvPath))
                {
                    pythonExe = venvPath;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{pythonScript}\"",
                    WorkingDirectory = aiStudioPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                pythonProcess = new Process { StartInfo = startInfo };
                pythonProcess.Exited += PythonProcess_Exited;
                pythonProcess.EnableRaisingEvents = true;
                
                pythonProcess.OutputDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                        Debug.WriteLine($"Python: {args.Data}");
                };
                
                pythonProcess.ErrorDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                        Debug.WriteLine($"Python Error: {args.Data}");
                };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();

                UpdateStatus($"Python app started (PID: {pythonProcess.Id})");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error launching Python app: {ex.Message}");
                MessageBox.Show($"Failed to launch Python application:\n{ex.Message}", 
                    "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopPythonApp()
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                try
                {
                    pythonProcess.Kill();
                    pythonProcess.WaitForExit(5000);
                    UpdateStatus("Python app stopped");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Error stopping Python app: {ex.Message}");
                }
            }
        }

        private void PythonProcess_Exited(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                UpdateStatus("Python app exited");
            });
        }

        private void StartMonitoring()
        {
            if (isMonitoring) return;

            logger = new MetricsLogger();
            metricsHistory.Clear();
            
            monitoringTimer.Interval = samplingRateMs;
            monitoringTimer.Start();
            
            isMonitoring = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            
            UpdateStatus($"Monitoring started. Log file: {logger.GetLogFilePath()}");
        }

        private void StopMonitoring()
        {
            if (!isMonitoring) return;

            monitoringTimer.Stop();
            logger?.Dispose();
            
            isMonitoring = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            
            UpdateStatus("Monitoring stopped");
        }

        private void MonitoringTimer_Tick(object sender, EventArgs e)
        {
            var metrics = monitor.GetCurrentMetrics();
            logger?.LogMetrics(metrics);
            
            metricsHistory.Add(metrics);
            if (metricsHistory.Count > MAX_HISTORY_POINTS)
            {
                metricsHistory.RemoveAt(0);
            }
            
            UpdateCharts();
            UpdateLabels(metrics);
        }

        private void UpdateCharts()
        {
            if (metricsHistory.Count == 0) return;

            double[] timePoints = metricsHistory.Select((m, i) => (double)i).ToArray();
            
            // CPU Chart
            cpuChart.Plot.Clear();
            double[] cpuValues = metricsHistory.Select(m => (double)m.CpuUsage).ToArray();
            cpuChart.Plot.Add.Scatter(timePoints, cpuValues);
            cpuChart.Refresh();
            
            // Memory Chart
            memoryChart.Plot.Clear();
            double[] memValues = metricsHistory.Select(m => (double)m.MemoryUsagePercent).ToArray();
            memoryChart.Plot.Add.Scatter(timePoints, memValues);
            memoryChart.Refresh();
            
            // GPU Chart
            gpuChart.Plot.Clear();
            double[] gpuValues = metricsHistory.Select(m => (double)m.GpuUsage).ToArray();
            gpuChart.Plot.Add.Scatter(timePoints, gpuValues);
            gpuChart.Refresh();
            
            // Disk I/O Chart
            diskChart.Plot.Clear();
            double[] diskReadValues = metricsHistory.Select(m => (double)m.DiskReadMBps).ToArray();
            double[] diskWriteValues = metricsHistory.Select(m => (double)m.DiskWriteMBps).ToArray();
            var readScatter = diskChart.Plot.Add.Scatter(timePoints, diskReadValues);
            readScatter.LegendText = "Read";
            var writeScatter = diskChart.Plot.Add.Scatter(timePoints, diskWriteValues);
            writeScatter.LegendText = "Write";
            writeScatter.Color = ScottPlot.Color.FromHex("#FF6B35");
            diskChart.Plot.ShowLegend();
            diskChart.Refresh();
        }

        private void UpdateLabels(SystemMetrics metrics)
        {
            lblCpu.Text = $"CPU: {metrics.CpuUsage:F1}%";
            lblMemory.Text = $"Memory: {metrics.MemoryUsageMB:F0} MB ({metrics.MemoryUsagePercent:F1}%)";
            lblGpu.Text = $"GPU: {metrics.GpuUsage:F1}% | {metrics.GpuMemoryUsedMB:F0} MB";
            lblDisk.Text = $"Disk: R:{metrics.DiskReadMBps:F2} MB/s | W:{metrics.DiskWriteMBps:F2} MB/s";
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = $"Status: {message}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopMonitoring();
            StopPythonApp();
            monitor?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
