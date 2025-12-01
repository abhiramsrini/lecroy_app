using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using LecroyScopeWinForms.Scope;

namespace LecroyScopeWinForms
{
    public partial class MainForm : Form
    {
        private const string CompanyBrand = "Your Company";
        private IScopeClient? _scopeClient;
        private bool _isBusy;

        public MainForm()
        {
            InitializeComponent();
            companyNameLabel.Text = CompanyBrand;
            scopeAddressTextBox.Text = "192.168.0.10";
            UpdateUiState();
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            var address = scopeAddressTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show(this, "Enter the oscilloscope address before connecting.", "Missing address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetBusy(true);
            await DisconnectClientAsync();
            _scopeClient = new ActiveDsoClient(dryRunCheckBox.Checked);

            try
            {
                await _scopeClient.ConnectAsync(address);
                AppendLog($"Connected to {address} {(dryRunCheckBox.Checked ? "(dry run)" : string.Empty)}");
                SetStatus(true, _scopeClient.IsDryRun);
                UpdateUiState();
            }
            catch (Exception ex)
            {
                AppendLog($"Connect failed: {ex.Message}");
                MessageBox.Show(this, ex.Message, "Connection failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await DisconnectClientAsync();
                SetStatus(false, dryRunCheckBox.Checked);
                UpdateUiState();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            SetBusy(true);
            await DisconnectClientAsync();
            SetStatus(false, dryRunCheckBox.Checked);
            UpdateUiState();
            SetBusy(false);
        }

        private async void SendCommandButton_Click(object sender, EventArgs e)
        {
            if (_isBusy || _scopeClient == null || !_scopeClient.IsConnected)
            {
                return;
            }

            var command = commandTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(command))
            {
                MessageBox.Show(this, "Enter a command to send.", "Command required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetBusy(true);
            responseTextBox.Text = "Sending...";

            try
            {
                AppendLog($"> {command}");
                var response = await _scopeClient.SendCommandAsync(command);
                responseTextBox.Text = response;
                if (!string.IsNullOrWhiteSpace(response))
                {
                    AppendLog($"< {response}");
                }
            }
            catch (Exception ex)
            {
                responseTextBox.Text = $"Error: {ex.Message}";
                AppendLog($"Command failed: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void LoadSetupButton_Click(object sender, EventArgs e)
        {
            if (_isBusy || _scopeClient == null || !_scopeClient.IsConnected)
            {
                return;
            }

            var setupPath = setupPathTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(setupPath))
            {
                MessageBox.Show(this, "Enter the setup file path on the scope.", "Setup path required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetBusy(true);
            AppendLog($"Loading setup: {setupPath}");
            try
            {
                await _scopeClient.LoadSetupAsync(setupPath);
                AppendLog("Setup loaded.");
                responseTextBox.Text = $"Setup loaded from {setupPath}";
                await RefreshMeasurementsAsync();
            }
            catch (Exception ex)
            {
                AppendLog($"Load setup failed: {ex.Message}");
                MessageBox.Show(this, ex.Message, "Load setup failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void CaptureButton_Click(object sender, EventArgs e)
        {
            await RefreshMeasurementsAsync();
        }

        private async Task DisconnectClientAsync()
        {
            if (_scopeClient == null)
            {
                return;
            }

            try
            {
                await _scopeClient.DisconnectAsync();
                AppendLog("Disconnected.");
            }
            catch
            {
                // Ignore cleanup issues to avoid blocking the UI.
            }
            finally
            {
                await _scopeClient.DisposeAsync();
                _scopeClient = null;
            }
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
            UpdateUiState();
        }

        private void SetStatus(bool connected, bool dryRun)
        {
            statusValueLabel.Text = connected ? (dryRun ? "Connected (dry run)" : "Connected") : "Disconnected";
            statusValueLabel.ForeColor = connected ? System.Drawing.Color.ForestGreen : System.Drawing.Color.Firebrick;
        }

        private void UpdateUiState()
        {
            var connected = _scopeClient?.IsConnected == true;
            connectButton.Enabled = !connected && !_isBusy;
            disconnectButton.Enabled = connected && !_isBusy;
            sendCommandButton.Enabled = connected && !_isBusy;
            loadSetupButton.Enabled = connected && !_isBusy;
            captureButton.Enabled = connected && !_isBusy;
            scopeAddressTextBox.Enabled = !connected;
            dryRunCheckBox.Enabled = !connected;
        }

        private void AppendLog(string message)
        {
            var line = $"{DateTime.Now:HH:mm:ss} {message}";
            if (logTextBox.TextLength == 0)
            {
                logTextBox.AppendText(line);
            }
            else
            {
                logTextBox.AppendText(Environment.NewLine + line);
            }
            logTextBox.ScrollToCaret();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DisconnectClientAsync().GetAwaiter().GetResult();
            }
            catch
            {
                // Closing, ignore cleanup errors.
            }
        }

        private async Task RefreshMeasurementsAsync()
        {
            if (_isBusy || _scopeClient == null || !_scopeClient.IsConnected)
            {
                return;
            }

            SetBusy(true);
            measurementsStatusLabel.Text = "Capturing...";

            try
            {
                var results = await _scopeClient.CaptureAndReadMeasurementsAsync();
                PopulateMeasurements(results);
                measurementsStatusLabel.Text = $"Last capture: {DateTime.Now:HH:mm:ss}";
                AppendLog("Capture and measurement read completed.");
            }
            catch (Exception ex)
            {
                measurementsStatusLabel.Text = "Capture failed";
                AppendLog($"Capture failed: {ex.Message}");
                MessageBox.Show(this, ex.Message, "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void PopulateMeasurements(IReadOnlyList<MeasurementResult> results)
        {
            measurementsGrid.Rows.Clear();
            foreach (var result in results)
            {
                measurementsGrid.Rows.Add(
                    result.Channel,
                    result.Name,
                    double.IsNaN(result.Value) ? "N/A" : result.Value.ToString("G6", CultureInfo.InvariantCulture),
                    result.Unit,
                    result.Timestamp.ToString("u"));
            }
        }
    }
}
