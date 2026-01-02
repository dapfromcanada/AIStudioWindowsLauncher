# AI Studio Windows Launcher

A C# Windows Forms application designed to launch and monitor Python-based AI applications with real-time system metrics tracking and visualization.

## Features

### 🚀 Python Application Launcher
- Automatically detects and launches Python applications from configurable paths
- Supports virtual environments (venv)
- Searches for common entry points: `main.py`, `app.py`, `run.py`, `__main__.py`

### 📊 Real-Time System Monitoring
- **CPU Usage**: Live percentage tracking
- **Memory Usage**: Total MB and percentage utilization
- **GPU Usage**: Auto-detects NVIDIA, AMD, and Intel GPUs
  - GPU utilization percentage
  - GPU memory usage (MB)
  - Supports nvidia-smi for NVIDIA GPUs
- **Disk I/O**: Read and write speeds in MB/s

### 📈 Live Charts
- Four interactive charts displaying real-time metrics:
  - CPU Usage (%)
  - Memory Usage (%)
  - GPU Usage (%)
  - Disk I/O (MB/s) with separate Read/Write lines
- Maintains up to 300 data points (5 minutes at 1-second sampling)

### 📝 CSV Data Logging
- Automatic timestamped CSV logging of all metrics
- Logs saved to: `Documents/AIStudioLauncher/Logs/`
- Format: `metrics_YYYYMMDD_HHMMSS.csv`
- Columns: Timestamp, CPU_Usage_%, Memory_Used_MB, Memory_Usage_%, GPU_Usage_%, GPU_Memory_Used_MB, Disk_Read_MBps, Disk_Write_MBps

### ⚙️ Configurable Settings
- **Python App Path**: Default `G:\AIStudio`, configurable via Settings dialog
- **Sampling Rate**: Default 1 second, configurable options:
  - 0.5s, 1s, 2s, 5s, 10s, 30s, 60s

## Requirements

- Windows 10 or later
- .NET 10.0
- Python 3.x (for the application you want to monitor)
- Optional: NVIDIA drivers with nvidia-smi (for NVIDIA GPU monitoring)

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/dapfromcanada/AIStudioWindowsLauncher.git
   ```

2. Build the project:
   ```bash
   cd AIStudioWindowsLauncher
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## Usage

1. **Configure Path**: Click "Settings" to set your AI application folder path
2. **Adjust Sampling**: Choose your preferred monitoring sampling rate
3. **Start Monitoring**: Click "Start" to launch the Python app and begin monitoring
4. **View Metrics**: Watch real-time charts and current metric values
5. **Stop**: Click "Stop" to end monitoring and close the Python application
6. **Check Logs**: Find detailed CSV logs in `Documents/AIStudioLauncher/Logs/`

## Technologies Used

- **C# / .NET 10.0**: Core application framework
- **Windows Forms**: UI framework
- **ScottPlot**: High-performance charting library
- **System.Management**: Windows performance counter access
- **System.Diagnostics**: Process management and monitoring

## GPU Detection

The application automatically detects your GPU type:
- **NVIDIA**: Uses nvidia-smi for detailed metrics
- **AMD/Intel**: Uses WMI performance counters

## Project Structure

```
AIStudioWindowsLauncher/
├── Form1.cs                      # Main application form
├── Form1.Designer.cs             # Form UI design
├── SettingsForm.cs               # Settings dialog
├── SettingsForm.Designer.cs      # Settings UI design
├── SystemMonitor.cs              # System metrics collection
├── MetricsLogger.cs              # CSV logging
├── Program.cs                    # Application entry point
└── AIStudioWindowsLauncher.csproj # Project configuration
```

## Future Enhancements

- Log file viewer/analyzer tool
- Historical data visualization
- Performance alerts and thresholds
- Multiple Python app profiles
- Export charts as images
- Network I/O monitoring

## License

This project is open source and available under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Author

**dapfromcanada**
- GitHub: [@dapfromcanada](https://github.com/dapfromcanada)
