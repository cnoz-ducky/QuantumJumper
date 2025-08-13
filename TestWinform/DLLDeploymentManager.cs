using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

public class DLLDeploymentManager
{
    private string sourceDllPath;
    private string targetDllPath;
    private string gameExecutablePath;
    private Process gameProcess;
    private bool isDeployed = false;

    public DLLDeploymentManager(string gameDirectory)
    {
        // Source DLL is in the launcher's directory
        sourceDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "quantumLeap.dll");

        // Target is the game directory
        targetDllPath = Path.Combine(gameDirectory, "version.dll");
        gameExecutablePath = Path.Combine(gameDirectory, "timewatch-win64-shipping.exe");
    }

    public async Task<bool> LaunchGameWithDLL()
    {
        try
        {
            // Step 1: Deploy the DLL
            if (!DeployDLL())
            {
                Console.WriteLine("Failed to deploy DLL");
                return false;
            }

            Console.WriteLine($"DLL deployed to: {targetDllPath}");

            // Step 2: Launch the game
            gameProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gameExecutablePath,
                    WorkingDirectory = Path.GetDirectoryName(gameExecutablePath),
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Set up event handler for when game exits
            gameProcess.Exited += OnGameExited;

            if (!gameProcess.Start())
            {
                Console.WriteLine("Failed to start game process");
                CleanupDLL();
                return false;
            }

            Console.WriteLine($"Game launched with PID: {gameProcess.Id}");

            // Step 3: Start monitoring the game process
            _ = Task.Run(() => MonitorGameProcess());

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error launching game: {ex.Message}");
            CleanupDLL();
            return false;
        }
    }

    private bool DeployDLL()
    {
        try
        {
            if (!File.Exists(sourceDllPath))
            {
                Console.WriteLine($"Source DLL not found: {sourceDllPath}");
                return false;
            }

            // Check if target already exists (from previous run)
            if (File.Exists(targetDllPath))
            {
                try
                {
                    File.Delete(targetDllPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not delete existing DLL: {ex.Message}");
                    // Try to continue anyway
                }
            }

            // Copy the DLL
            File.Copy(sourceDllPath, targetDllPath, true);
            isDeployed = true;

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to deploy DLL: {ex.Message}");
            return false;
        }
    }

    private async void MonitorGameProcess()
    {
        try
        {
            // Wait for the game process to exit
            await Task.Run(() => gameProcess?.WaitForExit());

            // Small delay to ensure game has fully closed
            await Task.Delay(1000);

            // Clean up the DLL
            CleanupDLL();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error monitoring game process: {ex.Message}");
            CleanupDLL();
        }
    }

    private void OnGameExited(object sender, EventArgs e)
    {
        Console.WriteLine("Game process has exited");
        CleanupDLL();
    }

    private void CleanupDLL()
    {
        if (!isDeployed) return;

        try
        {
            // Wait a moment for any file handles to be released
            Thread.Sleep(500);

            // Try to delete the deployed DLL
            if (File.Exists(targetDllPath))
            {
                // Retry logic in case file is still in use
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Delete(targetDllPath);
                        Console.WriteLine("DLL cleanup successful");
                        isDeployed = false;
                        break;
                    }
                    catch (IOException) when (i < 4)
                    {
                        // File might still be in use, wait and retry
                        Console.WriteLine($"DLL cleanup attempt {i + 1} failed, retrying...");
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DLL cleanup failed: {ex.Message}");
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during cleanup: {ex.Message}");
        }
    }

    // Manual cleanup method (call this when your launcher closes)
    public void ForceCleanup()
    {
        try
        {
            gameProcess?.Kill();
        }
        catch { }

        CleanupDLL();
    }

    // Check if game is still running
    public bool IsGameRunning()
    {
        try
        {
            return gameProcess != null && !gameProcess.HasExited;
        }
        catch
        {
            return false;
        }
    }
}