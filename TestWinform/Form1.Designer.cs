using quantumJumper.Properties;

namespace TestWinform
{
    partial class QuantumLauncher
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuantumLauncher));
            selectfileButton = new Button();
            gameFileTextBox = new TextBox();
            openFileDialog1 = new OpenFileDialog();
            gameFileLabel = new Label();
            launchButton = new Button();
            mapBox = new ComboBox();
            mapLabel = new Label();
            ipLabel = new Label();
            hostingToggleBox = new CheckBox();
            PortLabel = new Label();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            IPTextBox = new MaskedTextBox();
            connectionHelpButton = new Button();
            portTextBox = new MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // selectfileButton
            // 
            selectfileButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            selectfileButton.Location = new Point(589, 453);
            selectfileButton.Name = "selectfileButton";
            selectfileButton.Size = new Size(69, 23);
            selectfileButton.TabIndex = 0;
            selectfileButton.Text = "Select File";
            selectfileButton.UseVisualStyleBackColor = true;
            selectfileButton.Click += button1_Click;
            // 
            // gameFileTextBox
            // 
            gameFileTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gameFileTextBox.Location = new Point(12, 453);
            gameFileTextBox.Name = "gameFileTextBox";
            gameFileTextBox.Size = new Size(571, 23);
            gameFileTextBox.TabIndex = 2;
            gameFileTextBox.Text = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\TimeWatch\\TimeWatch\\Binaries\\Win64";
            gameFileTextBox.TextChanged += textBox1_TextChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.InitialDirectory = "G:\\SteamLibrary\\Steamapps\\common\\timewatch";
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            // 
            // gameFileLabel
            // 
            gameFileLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            gameFileLabel.AutoSize = true;
            gameFileLabel.Location = new Point(12, 435);
            gameFileLabel.Name = "gameFileLabel";
            gameFileLabel.Size = new Size(106, 15);
            gameFileLabel.TabIndex = 3;
            gameFileLabel.Text = "Game file Location";
            gameFileLabel.Click += label1_Click;
            // 
            // launchButton
            // 
            launchButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            launchButton.Location = new Point(730, 453);
            launchButton.Name = "launchButton";
            launchButton.Size = new Size(75, 23);
            launchButton.TabIndex = 4;
            launchButton.Text = "Launch";
            launchButton.UseVisualStyleBackColor = true;
            launchButton.Click += button2_Click;
            // 
            // mapBox
            // 
            mapBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            mapBox.DropDownStyle = ComboBoxStyle.DropDownList;
            mapBox.Enabled = false;
            mapBox.FormattingEnabled = true;
            mapBox.ImeMode = ImeMode.Off;
            mapBox.Items.AddRange(new object[] { "CargoShip", "ContainerYard", "MuseumArena", "NordicArena", "Overpass", "Overpass_Domination", "QuantumArena", "QuantumArenaNight", "QuantumStadium", "QuantumStadiumNight", "TutorialArena", "MainMenu" });
            mapBox.Location = new Point(628, 67);
            mapBox.Name = "mapBox";
            mapBox.Size = new Size(156, 23);
            mapBox.TabIndex = 5;
            mapBox.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // mapLabel
            // 
            mapLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            mapLabel.AutoSize = true;
            mapLabel.Location = new Point(628, 49);
            mapLabel.Name = "mapLabel";
            mapLabel.Size = new Size(31, 15);
            mapLabel.TabIndex = 8;
            mapLabel.Text = "Map";
            mapLabel.Click += label2_Click;
            // 
            // ipLabel
            // 
            ipLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ipLabel.AutoSize = true;
            ipLabel.Location = new Point(628, 196);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new Size(47, 15);
            ipLabel.TabIndex = 10;
            ipLabel.Text = "Enter IP";
            ipLabel.Click += label4_Click;
            // 
            // hostingToggleBox
            // 
            hostingToggleBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            hostingToggleBox.AutoSize = true;
            hostingToggleBox.Location = new Point(628, 12);
            hostingToggleBox.Name = "hostingToggleBox";
            hostingToggleBox.Size = new Size(68, 19);
            hostingToggleBox.TabIndex = 11;
            hostingToggleBox.Text = "Hosting";
            hostingToggleBox.UseVisualStyleBackColor = true;
            hostingToggleBox.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // PortLabel
            // 
            PortLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            PortLabel.AutoSize = true;
            PortLabel.Location = new Point(628, 240);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new Size(32, 15);
            PortLabel.TabIndex = 15;
            PortLabel.Text = "Port:";
            PortLabel.Click += label1_Click_1;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBox1.Location = new Point(618, -2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(198, 329);
            pictureBox1.TabIndex = 16;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 16);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 17;
            label1.Text = "QL Jumper by cnoz";
            // 
            // IPTextBox
            // 
            IPTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            IPTextBox.Culture = new System.Globalization.CultureInfo("");
            IPTextBox.Location = new Point(628, 214);
            IPTextBox.Name = "IPTextBox";
            IPTextBox.Size = new Size(100, 23);
            IPTextBox.TabIndex = 18;
            IPTextBox.Text = "127.0.0.1";
            IPTextBox.MaskInputRejected += IPTextBox_MaskInputRejected;
            IPTextBox.TextChanged += IPTextBox_TextChanged;
            // 
            // connectionHelpButton
            // 
            connectionHelpButton.AllowDrop = true;
            connectionHelpButton.Location = new Point(730, 8);
            connectionHelpButton.Name = "connectionHelpButton";
            connectionHelpButton.Size = new Size(75, 23);
            connectionHelpButton.TabIndex = 19;
            connectionHelpButton.Text = "Help";
            connectionHelpButton.UseVisualStyleBackColor = true;
            connectionHelpButton.Click += connectionHelpButton_Click;
            // 
            // portTextBox
            // 
            portTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            portTextBox.Location = new Point(628, 258);
            portTextBox.Mask = "0000";
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(31, 23);
            portTextBox.TabIndex = 14;
            portTextBox.Text = "7777";
            portTextBox.ValidatingType = typeof(int);
            portTextBox.MaskInputRejected += portTextBox_MaskInputRejected;
            portTextBox.TextChanged += portTextBox_TextChanged;
            // 
            // QuantumLauncher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(817, 488);
            Controls.Add(connectionHelpButton);
            Controls.Add(IPTextBox);
            Controls.Add(label1);
            Controls.Add(PortLabel);
            Controls.Add(portTextBox);
            Controls.Add(hostingToggleBox);
            Controls.Add(ipLabel);
            Controls.Add(mapLabel);
            Controls.Add(mapBox);
            Controls.Add(launchButton);
            Controls.Add(gameFileLabel);
            Controls.Add(gameFileTextBox);
            Controls.Add(selectfileButton);
            Controls.Add(pictureBox1);
            DoubleBuffered = true;
            Name = "QuantumLauncher";
            Text = "QuantumJumper";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void IPTextBox_TextChanged1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private Button selectfileButton;
        private TextBox gameFileTextBox;
        private OpenFileDialog openFileDialog1;
        private Label gameFileLabel;
        private Button launchButton;
        private ComboBox mapBox;
        private Label mapLabel;
        private Label ipLabel;
        private CheckBox hostingToggleBox;
        private Label PortLabel;
        private PictureBox pictureBox1;
        private Label label1;
        private MaskedTextBox IPTextBox;
        private MaskedTextBox portTextBox;
        private Button connectionHelpButton;
        //private DLLDeploymentManager dllManager;
    }
}
