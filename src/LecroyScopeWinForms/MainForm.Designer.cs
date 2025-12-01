using System.Drawing;
using System.Windows.Forms;

namespace LecroyScopeWinForms
{
    partial class MainForm
    {
        private Panel brandPanel;
        private Label companyNameLabel;
        private Label appTitleLabel;
        private GroupBox connectionGroupBox;
        private Label scopeAddressLabel;
        private TextBox scopeAddressTextBox;
        private Button connectButton;
        private Button disconnectButton;
        private Label statusLabel;
        private Label statusValueLabel;
        private CheckBox dryRunCheckBox;
        private GroupBox commandGroupBox;
        private Label commandLabel;
        private TextBox commandTextBox;
        private Button sendCommandButton;
        private Label responseLabel;
        private RichTextBox responseTextBox;
        private GroupBox logGroupBox;
        private RichTextBox logTextBox;

        private void InitializeComponent()
        {
            this.brandPanel = new Panel();
            this.companyNameLabel = new Label();
            this.appTitleLabel = new Label();
            this.connectionGroupBox = new GroupBox();
            this.scopeAddressLabel = new Label();
            this.scopeAddressTextBox = new TextBox();
            this.connectButton = new Button();
            this.disconnectButton = new Button();
            this.statusLabel = new Label();
            this.statusValueLabel = new Label();
            this.dryRunCheckBox = new CheckBox();
            this.commandGroupBox = new GroupBox();
            this.commandLabel = new Label();
            this.commandTextBox = new TextBox();
            this.sendCommandButton = new Button();
            this.responseLabel = new Label();
            this.responseTextBox = new RichTextBox();
            this.logGroupBox = new GroupBox();
            this.logTextBox = new RichTextBox();
            this.brandPanel.SuspendLayout();
            this.connectionGroupBox.SuspendLayout();
            this.commandGroupBox.SuspendLayout();
            this.logGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // brandPanel
            // 
            this.brandPanel.BackColor = Color.FromArgb(33, 150, 243);
            this.brandPanel.Controls.Add(this.companyNameLabel);
            this.brandPanel.Controls.Add(this.appTitleLabel);
            this.brandPanel.Dock = DockStyle.Top;
            this.brandPanel.Location = new Point(0, 0);
            this.brandPanel.Name = "brandPanel";
            this.brandPanel.Size = new Size(1042, 70);
            this.brandPanel.TabIndex = 0;
            // 
            // companyNameLabel
            // 
            this.companyNameLabel.AutoSize = true;
            this.companyNameLabel.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            this.companyNameLabel.ForeColor = Color.White;
            this.companyNameLabel.Location = new Point(22, 12);
            this.companyNameLabel.Name = "companyNameLabel";
            this.companyNameLabel.Size = new Size(149, 32);
            this.companyNameLabel.TabIndex = 0;
            this.companyNameLabel.Text = "Your Company";
            // 
            // appTitleLabel
            // 
            this.appTitleLabel.AutoSize = true;
            this.appTitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            this.appTitleLabel.ForeColor = Color.WhiteSmoke;
            this.appTitleLabel.Location = new Point(24, 40);
            this.appTitleLabel.Name = "appTitleLabel";
            this.appTitleLabel.Size = new Size(196, 23);
            this.appTitleLabel.TabIndex = 1;
            this.appTitleLabel.Text = "LeCroy Scope Controller";
            // 
            // connectionGroupBox
            // 
            this.connectionGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.connectionGroupBox.Controls.Add(this.dryRunCheckBox);
            this.connectionGroupBox.Controls.Add(this.statusValueLabel);
            this.connectionGroupBox.Controls.Add(this.statusLabel);
            this.connectionGroupBox.Controls.Add(this.disconnectButton);
            this.connectionGroupBox.Controls.Add(this.connectButton);
            this.connectionGroupBox.Controls.Add(this.scopeAddressTextBox);
            this.connectionGroupBox.Controls.Add(this.scopeAddressLabel);
            this.connectionGroupBox.Location = new Point(16, 86);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new Size(308, 214);
            this.connectionGroupBox.TabIndex = 1;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "Connection";
            // 
            // scopeAddressLabel
            // 
            this.scopeAddressLabel.AutoSize = true;
            this.scopeAddressLabel.Location = new Point(14, 35);
            this.scopeAddressLabel.Name = "scopeAddressLabel";
            this.scopeAddressLabel.Size = new Size(146, 20);
            this.scopeAddressLabel.TabIndex = 0;
            this.scopeAddressLabel.Text = "Scope Address / IP:";
            // 
            // scopeAddressTextBox
            // 
            this.scopeAddressTextBox.Location = new Point(18, 58);
            this.scopeAddressTextBox.Name = "scopeAddressTextBox";
            this.scopeAddressTextBox.PlaceholderText = "e.g., 192.168.0.10";
            this.scopeAddressTextBox.Size = new Size(270, 27);
            this.scopeAddressTextBox.TabIndex = 1;
            // 
            // connectButton
            // 
            this.connectButton.Location = new Point(18, 138);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new Size(94, 29);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new Point(121, 138);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new Size(104, 29);
            this.disconnectButton.TabIndex = 3;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new Point(18, 181);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(52, 20);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Status:";
            // 
            // statusValueLabel
            // 
            this.statusValueLabel.AutoSize = true;
            this.statusValueLabel.ForeColor = Color.Firebrick;
            this.statusValueLabel.Location = new Point(76, 181);
            this.statusValueLabel.Name = "statusValueLabel";
            this.statusValueLabel.Size = new Size(96, 20);
            this.statusValueLabel.TabIndex = 5;
            this.statusValueLabel.Text = "Disconnected";
            // 
            // dryRunCheckBox
            // 
            this.dryRunCheckBox.AutoSize = true;
            this.dryRunCheckBox.Location = new Point(18, 101);
            this.dryRunCheckBox.Name = "dryRunCheckBox";
            this.dryRunCheckBox.Size = new Size(167, 24);
            this.dryRunCheckBox.TabIndex = 6;
            this.dryRunCheckBox.Text = "Dry run (no hardware)";
            this.dryRunCheckBox.UseVisualStyleBackColor = true;
            // 
            // commandGroupBox
            // 
            this.commandGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.commandGroupBox.Controls.Add(this.responseTextBox);
            this.commandGroupBox.Controls.Add(this.responseLabel);
            this.commandGroupBox.Controls.Add(this.sendCommandButton);
            this.commandGroupBox.Controls.Add(this.commandTextBox);
            this.commandGroupBox.Controls.Add(this.commandLabel);
            this.commandGroupBox.Location = new Point(338, 86);
            this.commandGroupBox.Name = "commandGroupBox";
            this.commandGroupBox.Size = new Size(692, 214);
            this.commandGroupBox.TabIndex = 2;
            this.commandGroupBox.TabStop = false;
            this.commandGroupBox.Text = "Commands";
            // 
            // commandLabel
            // 
            this.commandLabel.AutoSize = true;
            this.commandLabel.Location = new Point(20, 35);
            this.commandLabel.Name = "commandLabel";
            this.commandLabel.Size = new Size(132, 20);
            this.commandLabel.TabIndex = 0;
            this.commandLabel.Text = "Custom Command:";
            // 
            // commandTextBox
            // 
            this.commandTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.commandTextBox.Location = new Point(24, 58);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.PlaceholderText = "e.g., C1:VDIV 500mV";
            this.commandTextBox.Size = new Size(527, 27);
            this.commandTextBox.TabIndex = 1;
            // 
            // sendCommandButton
            // 
            this.sendCommandButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.sendCommandButton.Enabled = false;
            this.sendCommandButton.Location = new Point(566, 56);
            this.sendCommandButton.Name = "sendCommandButton";
            this.sendCommandButton.Size = new Size(108, 29);
            this.sendCommandButton.TabIndex = 2;
            this.sendCommandButton.Text = "Send";
            this.sendCommandButton.UseVisualStyleBackColor = true;
            this.sendCommandButton.Click += new System.EventHandler(this.SendCommandButton_Click);
            // 
            // responseLabel
            // 
            this.responseLabel.AutoSize = true;
            this.responseLabel.Location = new Point(20, 97);
            this.responseLabel.Name = "responseLabel";
            this.responseLabel.Size = new Size(77, 20);
            this.responseLabel.TabIndex = 3;
            this.responseLabel.Text = "Response:";
            // 
            // responseTextBox
            // 
            this.responseTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.responseTextBox.Location = new Point(24, 120);
            this.responseTextBox.Name = "responseTextBox";
            this.responseTextBox.ReadOnly = true;
            this.responseTextBox.Size = new Size(650, 76);
            this.responseTextBox.TabIndex = 4;
            this.responseTextBox.Text = "";
            // 
            // logGroupBox
            // 
            this.logGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.logGroupBox.Controls.Add(this.logTextBox);
            this.logGroupBox.Location = new Point(16, 316);
            this.logGroupBox.Name = "logGroupBox";
            this.logGroupBox.Size = new Size(1014, 330);
            this.logGroupBox.TabIndex = 3;
            this.logGroupBox.TabStop = false;
            this.logGroupBox.Text = "Activity Log";
            // 
            // logTextBox
            // 
            this.logTextBox.Dock = DockStyle.Fill;
            this.logTextBox.Location = new Point(3, 23);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new Size(1008, 304);
            this.logTextBox.TabIndex = 0;
            this.logTextBox.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1042, 664);
            this.Controls.Add(this.logGroupBox);
            this.Controls.Add(this.commandGroupBox);
            this.Controls.Add(this.connectionGroupBox);
            this.Controls.Add(this.brandPanel);
            this.MinimumSize = new Size(900, 550);
            this.Name = "MainForm";
            this.Text = "LeCroy Scope Controller";
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.brandPanel.ResumeLayout(false);
            this.brandPanel.PerformLayout();
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.commandGroupBox.ResumeLayout(false);
            this.commandGroupBox.PerformLayout();
            this.logGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
