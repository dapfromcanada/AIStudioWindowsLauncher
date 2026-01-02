using System;
using System.IO;
using System.Text;

namespace AIStudioWindowsLauncher
{
    public class MetricsLogger : IDisposable
    {
        private string logFilePath;
        private StreamWriter writer;
        private bool isHeaderWritten = false;

        public MetricsLogger(string? logDirectory = null)
        {
            if (string.IsNullOrEmpty(logDirectory))
            {
                logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "AIStudioLauncher", "Logs");
            }

            Directory.CreateDirectory(logDirectory);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logFilePath = Path.Combine(logDirectory, $"metrics_{timestamp}.csv");

            writer = new StreamWriter(logFilePath, false, Encoding.UTF8);
            WriteHeader();
        }

        private void WriteHeader()
        {
            writer.WriteLine("Timestamp,CPU_Usage_%,Memory_Used_MB,Memory_Usage_%,GPU_Usage_%,GPU_Memory_Used_MB,Disk_Read_MBps,Disk_Write_MBps");
            writer.Flush();
            isHeaderWritten = true;
        }

        public void LogMetrics(SystemMetrics metrics)
        {
            if (writer == null || !isHeaderWritten)
                return;

            try
            {
                string line = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2},{3:F2},{4:F2},{5:F2},{6:F2},{7:F2}",
                    metrics.Timestamp,
                    metrics.CpuUsage,
                    metrics.MemoryUsageMB,
                    metrics.MemoryUsagePercent,
                    metrics.GpuUsage,
                    metrics.GpuMemoryUsedMB,
                    metrics.DiskReadMBps,
                    metrics.DiskWriteMBps);

                writer.WriteLine(line);
                writer.Flush();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error writing to log: {ex.Message}");
            }
        }

        public string GetLogFilePath()
        {
            return logFilePath;
        }

        public void Dispose()
        {
            writer?.Flush();
            writer?.Close();
            writer?.Dispose();
        }
    }
}
