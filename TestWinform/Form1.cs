using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Web;
using System.Windows.Forms.VisualStyles;
using Reloaded;
using Reloaded.Injector;
using System.Runtime.CompilerServices;
using quantumJumper.Properties;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace TestWinform
{
    public partial class QuantumLauncher : Form
    {
        private string executable_path;
        private LauncherSettings settings;
        private DLLDeploymentManager dllManager;
        private bool isFirstLoad = true;

        public QuantumLauncher()
        {
            InitializeComponent();

            // Load settings first
            settings = LauncherSettings.Load();
            settings.AutoDetectGameDirectory();

            this.FormClosing += OnLauncherClosing;
            this.Load += QuantumLauncher_Load;
            this.Shown += QuantumLauncher_Shown; // Add this event for when form is fully visible
        }

        private void QuantumLauncher_Shown(object sender, EventArgs e)
        {
            // This fires after the form is completely loaded and visible
            // Safe place to apply settings if Load event is still too early
            if (isFirstLoad)
            {
                ApplySettingsToUI();

                if (string.IsNullOrEmpty(settings.GameDirectory) || !IsValidGameDirectory(settings.GameDirectory))
                {
                    ShowFirstTimeSetup();
                }

                isFirstLoad = false;
            }
        }

        private void QuantumLauncher_Load(object sender, EventArgs e)
        {
            // Apply saved settings to UI (after controls are initialized)
            ApplySettingsToUI();

            // Check if this is first time setup
            if (string.IsNullOrEmpty(settings.GameDirectory) || !IsValidGameDirectory(settings.GameDirectory))
            {
                // Use BeginInvoke to ensure the form is fully loaded before showing dialogs
                this.BeginInvoke(new Action(() => ShowFirstTimeSetup()));
            }

            isFirstLoad = false;
        }

        private void ApplySettingsToUI()
        {
            try
            {
                if (!string.IsNullOrEmpty(settings.GameDirectory) && IsValidGameDirectory(settings.GameDirectory))
                {
                    executable_path = Path.Combine(settings.GameDirectory, "TimeWatch-Win64-Shipping.exe");
                    if (gameFileTextBox != null)
                    {
                        gameFileTextBox.Text = executable_path;
                    }
                    dllManager = new DLLDeploymentManager(settings.GameDirectory);
                }

                // Apply other saved settings with null checks
                if (mapBox != null)
                    mapBox.Text = settings.LastUsedMap;

                if (IPTextBox != null)
                    IPTextBox.Text = settings.PlayerIP;

                if (hostingToggleBox != null)
                    hostingToggleBox.Checked = settings.IsHost;

                if (portTextBox != null)
                    portTextBox.Text = settings.ServerPort.ToString();

                // Update UI state based on hosting toggle
                UpdateHostingToggleUI();
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the launcher
                System.Diagnostics.Debug.WriteLine($"Error applying settings to UI: {ex.Message}");
            }
        }

        private void SaveSettingsFromUI()
        {
            if (!isFirstLoad) // Don't save during initial load
            {
                settings.GameDirectory = string.IsNullOrEmpty(executable_path) ? "" : Path.GetDirectoryName(executable_path);
                settings.LastUsedMap = mapBox.Text;
                settings.PlayerIP = IPTextBox.Text;
                settings.IsHost = hostingToggleBox.Checked;
                if (int.TryParse(portTextBox.Text, out int port))
                {
                    settings.ServerPort = port;
                }

                settings.Save();
            }
        }

        private bool IsValidGameDirectory(string directory)
        {
            return !string.IsNullOrEmpty(directory) &&
                   Directory.Exists(directory) &&
                   File.Exists(Path.Combine(directory, "TimeWatch-Win64-Shipping.exe"));
        }

        private void ShowFirstTimeSetup()
        {
            var message = "Welcome to Quantum League Launcher!\n\n";

            if (!string.IsNullOrEmpty(settings.GameDirectory))
            {
                message += $"Game detected at: {settings.GameDirectory}\n\n";
                message += "If this is correct, click OK. Otherwise, click Cancel to browse for the game directory.";

                var result = MessageBox.Show(message, "First Time Setup", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    return; // Use auto-detected path
                }
            }
            else
            {
                message += "Could not automatically detect Quantum League installation.\n";
                message += "Please select the game directory.";
                MessageBox.Show(message, "First Time Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Show browse dialog
            button1_Click(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Executable Files|*.exe|All Files|*.*";
            file.Title = "Select TimeWatch-Win64-Shipping.exe";

            // Start from saved directory if available
            if (!string.IsNullOrEmpty(settings.GameDirectory))
            {
                file.InitialDirectory = settings.GameDirectory;
            }

            if (file.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetFileName(file.FileName).Equals("TimeWatch-Win64-Shipping.exe", StringComparison.OrdinalIgnoreCase))
                {
                    executable_path = file.FileName;
                    gameFileTextBox.Text = executable_path;

                    // Update settings
                    settings.GameDirectory = Path.GetDirectoryName(executable_path);
                    settings.Save();

                    // Initialize DLL manager
                    dllManager = new DLLDeploymentManager(settings.GameDirectory);

                    MessageBox.Show("Game directory saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select the correct executable: TimeWatch-Win64-Shipping.exe", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // This is handled in QuantumLauncher_Load now
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Auto-save when text changes (but not during initial load)
            SaveSettingsFromUI();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Keep this empty - handled in button1_Click
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Keep empty
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(executable_path) || !File.Exists(executable_path))
            {
                MessageBox.Show("Please select a valid game executable first.", "No Game Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                launchButton.Enabled = false;
                launchButton.Text = "Launching...";

                string executabledir = Path.GetDirectoryName(executable_path);

                // Validate inputs
                string ip = IPTextBox.Text;
                bool hosting = hostingToggleBox.Checked;

                if (!hosting && string.IsNullOrWhiteSpace(ip))
                {
                    MessageBox.Show("IP address is required when not hosting.", "IP Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ResetLaunchButton();
                    return;
                }

                string map = mapBox.Text;
                if (hosting && string.IsNullOrWhiteSpace(map))
                {
                    MessageBox.Show("Map selection is required when hosting.", "Map Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ResetLaunchButton();
                    return;
                }

                // Write config
                string configPath = Path.Combine(executabledir, "config.txt");
                ConfigWriter config = new ConfigWriter(configPath);

                config.SetValue("Port", portTextBox.Text);
                config.SetGameSettings(map, hosting, ip);
                config.WriteConfigFile();

                // Save current settings
                SaveSettingsFromUI();

                // Show launch info
                string launchInfo = $"Launching with:\nIP: {ip}\nMap: {map}\nHosting: {hosting}\nPort: {portTextBox.Text}";
                //var result = MessageBox.Show(launchInfo + "\n\nProceed with launch?", "Launch Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                //if (result != DialogResult.Yes)
                //{
                //    ResetLaunchButton();
                //    return;
                //}

                // Deploy dll and launch
                if (dllManager == null)
                {
                    dllManager = new DLLDeploymentManager(executabledir);
                }

                bool success = await dllManager.LaunchGameWithDLL();

                if (success)
                {
                    launchButton.Text = "Game Running";

                    // Monitor game process
                    _ = Task.Run(async () =>
                    {
                        while (dllManager.IsGameRunning())
                        {
                            await Task.Delay(1000);
                        }

                        // Game has exited, re-enable button on UI thread
                        this.Invoke((Action)(() =>
                        {
                            ResetLaunchButton();
                        }));
                    });
                }
                else
                {
                    MessageBox.Show("Failed to launch game. Check that the game directory is correct and try running as administrator.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetLaunchButton();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Launch error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLaunchButton();
            }
        }

        private void ResetLaunchButton()
        {
            launchButton.Enabled = true;
            launchButton.Text = "Launch Game";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettingsFromUI();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHostingToggleUI();
            // SaveSettingsFromUI();
        }

        private void UpdateHostingToggleUI()
        {
            try
            {
                if (hostingToggleBox?.Checked == false)
                {
                    if (mapLabel != null) mapLabel.Enabled = false;
                    if (mapBox != null) mapBox.Enabled = false;
                    if (ipLabel != null) ipLabel.Enabled = true;
                    if (IPTextBox != null) IPTextBox.Enabled = true;
                    if (portTextBox != null) IPTextBox.Enabled = true;
                    // Show P2P connection tip for clients
                    this.Text = "Quantum League Launcher - Client Mode";
                }
                else if (hostingToggleBox?.Checked == true)
                {
                    if (mapLabel != null) mapLabel.Enabled = true;
                    if (ipLabel != null) ipLabel.Enabled = false;
                    if (IPTextBox != null) IPTextBox.Enabled = false;
                    if (portTextBox != null) IPTextBox.Enabled = true;
                    if (mapBox != null) mapBox.Enabled = true;

                    // Show hosting tip
                    this.Text = "Quantum League Launcher - Host Mode";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating hosting toggle UI: {ex.Message}");
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            // Keep empty
        }

        private void portTextBox_TextChanged(object sender, EventArgs e)
        {
            SaveSettingsFromUI();
        }

        private void IPTextBox_TextChanged(object sender, EventArgs e)
        {
            SaveSettingsFromUI();
        }

        private void IPTextBox_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            // Keep empty
        }

        private void OnLauncherClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsFromUI();
            dllManager?.ForceCleanup();
        }

        // Add a menu item or button for advanced settings
        private void ShowAdvancedSettings()
        {
            // TODO: Implement advanced settings form later
            MessageBox.Show("Advanced settings coming soon!", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Add this method to show P2P connection help
        private void ShowP2PHelp()
        {
            string helpText = @"P2P Connection Help:

For HOST (the person creating the game):
1. Forward port 7777 on your router to your computer
2. Share your PUBLIC IP address with other players
3. Make sure Windows Firewall allows the game

For CLIENT (joining a game):
1. Get the host's PUBLIC IP address
2. Enter it in the IP field
3. Make sure the port matches (default: 7777)

Alternative Solutions:
• Use Hamachi/Radmin VPN for virtual LAN
• Try UPnP port forwarding (if router supports it)
• Host can try enabling DMZ mode temporarily

Need help? Check the connection troubleshooting guide!";

            MessageBox.Show(helpText, "P2P Connection Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void connectionHelpButton_Click(object sender, EventArgs e)
        {
            ShowP2PHelp();
        }

        private void portTextBox_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
    }

    // TODO: Implement AdvancedSettingsForm later if needed
    /*
    public partial class AdvancedSettingsForm : Form
    {
        public LauncherSettings UpdatedSettings { get; private set; }

        public AdvancedSettingsForm(LauncherSettings currentSettings)
        {
            InitializeComponent();
            UpdatedSettings = currentSettings;
            
            // Load current settings into form controls
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Add controls for additional settings like:
            // - Remember window position
            // - Auto-detect game updates  
            // - Default player name
            // - Connection timeout values
            // - etc.
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Save advanced settings
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
    */
}

// Add this class to your project for the settings system
public class LauncherSettings
{
    public string GameDirectory { get; set; } = "";
    public string LastUsedMap { get; set; } = "CargoShip";
    public string PlayerIP { get; set; } = "127.0.0.1";
    public bool IsHost { get; set; } = true;
    public int ServerPort { get; set; } = 7777;
    public string PlayerName { get; set; } = "";
    public bool RememberWindowPosition { get; set; } = true;
    public int WindowX { get; set; } = -1;
    public int WindowY { get; set; } = -1;
    public bool AutoDetectUpdates { get; set; } = false;

    public void Save()
    {
        try
        {
            string settingsPath = GetSettingsPath();
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    public static LauncherSettings Load()
    {
        try
        {
            string settingsPath = GetSettingsPath();
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                var settings = JsonConvert.DeserializeObject<LauncherSettings>(json);
                return settings ?? new LauncherSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }

        return new LauncherSettings();
    }

    private static string GetSettingsPath()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder = Path.Combine(appData, "QuantumLeague Launcher");
        Directory.CreateDirectory(appFolder);
        return Path.Combine(appFolder, "settings.json");
    }

    public void AutoDetectGameDirectory()
    {
        if (string.IsNullOrEmpty(GameDirectory) || !Directory.Exists(GameDirectory))
        {
            GameDirectory = FindQuantumLeagueDirectory() ?? "";
        }
    }

    private string FindQuantumLeagueDirectory()
    {
        string[] steamPaths = {
            @"C:\Program Files (x86)\Steam\steamapps\common\Quantum League",
            @"C:\Program Files\Steam\steamapps\common\Quantum League",
            @"D:\Steam\steamapps\common\Quantum League",
            @"E:\Steam\steamapps\common\Quantum League"
        };

        string[] epicPaths = {
            @"C:\Program Files\Epic Games\QuantumLeague",
            @"C:\Program Files (x86)\Epic Games\QuantumLeague"
        };

        foreach (string path in steamPaths.Concat(epicPaths))
        {
            if (Directory.Exists(path) && File.Exists(Path.Combine(path, "TimeWatch-Win64-Shipping.exe")))
            {
                return path;
            }
        }

        return null;
    }
}