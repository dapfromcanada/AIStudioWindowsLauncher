using System;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Linq;

namespace AIStudioWindowsLauncher
{
    public class SystemMetrics
    {
        public DateTime Timestamp { get; set; }
        public float CpuUsage { get; set; }
        public float MemoryUsageMB { get; set; }
        public float MemoryUsagePercent { get; set; }
        public float GpuUsage { get; set; }
        public float GpuMemoryUsedMB { get; set; }
        public float DiskReadMBps { get; set; }
        public float DiskWriteMBps { get; set; }
    }

    public class SystemMonitor : IDisposable
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;
        private PerformanceCounter diskReadCounter;
        private PerformanceCounter diskWriteCounter;
        
        private string gpuType = "Unknown";
        private ManagementObject gpuObject = null;
        private bool hasNvidiaGpu = false;
        private bool hasAmdGpu = false;
        private bool hasIntelGpu = false;

        public string GpuType => gpuType;

        public SystemMonitor()
        {
            InitializePerformanceCounters();
            DetectGpu();
        }

        private void InitializePerformanceCounters()
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
                
                // Try to get physical disk counters (may need to be adjusted based on system)
                try
                {
                    diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                    diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
                }
                catch
                {
                    // If physical disk counters fail, try logical disk
                    diskReadCounter = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "_Total");
                    diskWriteCounter = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "_Total");
                }
                
                // Initial read to initialize counters
                cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing performance counters: {ex.Message}");
            }
        }

        private void DetectGpu()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        string vendor = obj["AdapterCompatibility"]?.ToString() ?? "";
                        
                        if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
                        {
                            hasNvidiaGpu = true;
                            gpuType = "NVIDIA";
                            gpuObject = obj;
                            break;
                        }
                        else if (name.Contains("AMD", StringComparison.OrdinalIgnoreCase) || 
                                 name.Contains("Radeon", StringComparison.OrdinalIgnoreCase))
                        {
                            hasAmdGpu = true;
                            gpuType = "AMD";
                            gpuObject = obj;
                            break;
                        }
                        else if (name.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                        {
                            hasIntelGpu = true;
                            gpuType = "Intel";
                            gpuObject = obj;
                            break;
                        }
                    }
                }
                
                Debug.WriteLine($"Detected GPU: {gpuType}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error detecting GPU: {ex.Message}");
                gpuType = "Unknown";
            }
        }

        public SystemMetrics GetCurrentMetrics()
        {
            var metrics = new SystemMetrics
            {
                Timestamp = DateTime.Now
            };

            try
            {
                // CPU Usage
                metrics.CpuUsage = cpuCounter?.NextValue() ?? 0;

                // Memory Usage
                float availableMemoryMB = memoryCounter?.NextValue() ?? 0;
                float totalMemoryMB = GetTotalMemoryMB();
                metrics.MemoryUsageMB = totalMemoryMB - availableMemoryMB;
                metrics.MemoryUsagePercent = totalMemoryMB > 0 ? (metrics.MemoryUsageMB / totalMemoryMB) * 100 : 0;

                // Disk I/O (convert bytes/sec to MB/sec)
                metrics.DiskReadMBps = (diskReadCounter?.NextValue() ?? 0) / (1024 * 1024);
                metrics.DiskWriteMBps = (diskWriteCounter?.NextValue() ?? 0) / (1024 * 1024);

                // GPU Usage (basic implementation)
                GetGpuMetrics(ref metrics);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error collecting metrics: {ex.Message}");
            }

            return metrics;
        }

        private void GetGpuMetrics(ref SystemMetrics metrics)
        {
            try
            {
                if (hasNvidiaGpu)
                {
                    // For NVIDIA, try to use nvidia-smi if available
                    GetNvidiaGpuMetrics(ref metrics);
                }
                else if (hasAmdGpu || hasIntelGpu)
                {
                    // For AMD/Intel, use WMI performance counters if available
                    GetGenericGpuMetrics(ref metrics);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting GPU metrics: {ex.Message}");
                metrics.GpuUsage = 0;
                metrics.GpuMemoryUsedMB = 0;
            }
        }

        private void GetNvidiaGpuMetrics(ref SystemMetrics metrics)
        {
            try
            {
                // Try using nvidia-smi command
                var startInfo = new ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    Arguments = "--query-gpu=utilization.gpu,memory.used --format=csv,noheader,nounits",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
                        {
                            var parts = output.Trim().Split(',');
                            if (parts.Length >= 2)
                            {
                                float.TryParse(parts[0].Trim(), out float gpuUsage);
                                float.TryParse(parts[1].Trim(), out float gpuMemory);
                                
                                metrics.GpuUsage = gpuUsage;
                                metrics.GpuMemoryUsedMB = gpuMemory;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"nvidia-smi not available or failed: {ex.Message}");
            }

            // Fallback to generic method
            GetGenericGpuMetrics(ref metrics);
        }

        private void GetGenericGpuMetrics(ref SystemMetrics metrics)
        {
            try
            {
                // Try to get GPU usage from performance counters
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_GPUPerformanceCounters_GPUEngine"))
                {
                    var results = searcher.Get();
                    if (results.Count > 0)
                    {
                        double totalUtilization = 0;
                        int count = 0;
                        
                        foreach (ManagementObject obj in results)
                        {
                            var utilization = obj["UtilizationPercentage"];
                            if (utilization != null)
                            {
                                totalUtilization += Convert.ToDouble(utilization);
                                count++;
                            }
                        }
                        
                        if (count > 0)
                        {
                            metrics.GpuUsage = (float)(totalUtilization / count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic GPU metrics failed: {ex.Message}");
                metrics.GpuUsage = 0;
                metrics.GpuMemoryUsedMB = 0;
            }
        }

        private float GetTotalMemoryMB()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        ulong totalBytes = (ulong)obj["TotalPhysicalMemory"];
                        return totalBytes / (1024f * 1024f);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting total memory: {ex.Message}");
            }
            return 0;
        }

        public void Dispose()
        {
            cpuCounter?.Dispose();
            memoryCounter?.Dispose();
            diskReadCounter?.Dispose();
            diskWriteCounter?.Dispose();
            gpuObject?.Dispose();
        }
    }
}
