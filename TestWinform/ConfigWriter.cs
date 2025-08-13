using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace TestWinform
{

    // Simple config writer class for Quantum League launcher
    public class ConfigWriter
    {
        private string configPath;
        private Dictionary<string, object> configValues;

        public ConfigWriter(string filePath = "config.txt")
        {
            configPath = filePath;
            configValues = new Dictionary<string, object>();
        }

        // Add configuration values
        public void SetValue(string key, string value)
        {
            configValues[key] = value;
        }

        public void SetValue(string key, bool value)
        {
            configValues[key] = value;
        }

        public void SetValue(string key, float value)
        {
            configValues[key] = value;
        }

        public void SetValue(string key, int value)
        {
            configValues[key] = (float)value; // Convert to float for consistency
        }

        // Write all config values to file
        public void WriteConfigFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
                {
                    // Write header comment
                    //writer.WriteLine("#==================================================");
                    //writer.WriteLine("# Quantum League Launcher Configuration");
                    //writer.WriteLine("# Generated automatically - modify through launcher");
                    //writer.WriteLine("#==================================================");
                    //writer.WriteLine();

                    // Write each config value
                    foreach (var kvp in configValues)
                    {
                        string formattedValue = FormatValue(kvp.Value);
                        writer.WriteLine($"{kvp.Key} = {formattedValue}");
                    }
                }

                Console.WriteLine($"Config file written successfully to: {configPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing config file: {ex.Message}");
                throw;
            }
        }

        // Format values according to your DLL's expected format
        private string FormatValue(object value)
        {
            switch (value)
            {
                case bool boolValue:
                    return boolValue ? "true" : "false";

                case string stringValue:
                    // Add the "A_" prefix for IP addresses if needed
                    if (configValues.ContainsKey("IP") && value == configValues["IP"])
                    {
                        return $"A_{stringValue}";
                    }
                    return stringValue;

                case float floatValue:
                    return floatValue.ToString("F2"); // 2 decimal places

                default:
                    return value.ToString();
            }
        }

        // Convenience method to set all common game settings at once
        public void SetGameSettings(string mapName, bool isHost, string ipAddress)
        {
            SetValue("Map", mapName);
            SetValue("Host", isHost);
            SetValue("IP", ipAddress);
        }

        // Method to add custom settings
        public void AddCustomSetting(string key, object value)
        {
            SetValue(key, FormatValue(value));
        }
    }
}
    // Example usage class showing how to use the ConfigWriter
    //public class LauncherExample
    //{
    //    public static void ExampleUsage()
    //    {
    //        // Create config writer
    //        ConfigWriter config = new ConfigWriter("config.txt");

    //        // Example 1: Host setup
    //        config.SetGameSettings("QuantumArena", true, "127.0.0.1");
    //        config.WriteConfigFile();

    //        // Example 2: Client setup
    //        ConfigWriter clientConfig = new ConfigWriter("config.txt");
    //        clientConfig.SetGameSettings("QuantumArena", false, "192.168.1.100");
    //        clientConfig.WriteConfigFile();

    //        // Example 3: Advanced setup with custom settings
    //        ConfigWriter advancedConfig = new ConfigWriter("config.txt");
    //        advancedConfig.SetValue("Map", "QuantumArenaNight");
    //        advancedConfig.SetValue("Host", true);
    //        advancedConfig.SetValue("IP", "127.0.0.1");
    //        advancedConfig.SetValue("MaxPlayers", 8);
    //        advancedConfig.SetValue("GameMode", "Deathmatch");
    //        advancedConfig.SetValue("TimeLimit", 10.5f);
    //        advancedConfig.WriteConfigFile();
    //    }
    //}

    //// If you're using Windows Forms or WPF, here's how to integrate it:
    //public class LauncherForm
    //{
    //    // Example form variables (replace with your actual UI controls)
    //    private string selectedMap = "QuantumArena";
    //    private bool hostMode = true;
    //    private string targetIP = "127.0.0.1";

    //    // Method to call when user clicks "Launch Game"
    //    private void LaunchGame_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            // Step 1: Generate config file from UI values
    //            ConfigWriter config = new ConfigWriter();
    //            config.SetGameSettings(selectedMap, hostMode, targetIP);
    //            config.WriteConfigFile();

    //            // Step 2: Launch the game (your existing game launch code)
    //            LaunchQuantumLeague();

    //            // Step 3: Inject DLL (we'll work on this next)
    //            // InjectDLL();

    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show($"Error launching game: {ex.Message}", "Launch Error");
    //        }
    //    }

    //    private void LaunchQuantumLeague()
    //    {
    //    // Your game launching code here
    //    System.Diagnostics.Process.Start("QuantumLeague.exe");
    //    }
    //}
