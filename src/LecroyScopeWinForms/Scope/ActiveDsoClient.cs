using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LecroyScopeWinForms.Scope
{
    public sealed class ActiveDsoClient : IScopeClient
    {
        private readonly bool _dryRun;
        private dynamic? _scope;
        private bool _connected;
        private bool _measurementsConfigured;

        public ActiveDsoClient(bool dryRun = false)
        {
            _dryRun = dryRun;
        }

        public bool IsConnected => _connected;
        public bool IsDryRun => _dryRun;

        public async Task ConnectAsync(string address, CancellationToken cancellationToken = default)
        {
            if (_connected)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address is required.", nameof(address));
            }

            if (_dryRun)
            {
                _connected = true;
                return;
            }

            await Task.Run(() =>
            {
                var progType = Type.GetTypeFromProgID("LeCroy.ActiveDSOCtrl.1") ??
                               Type.GetTypeFromProgID("LeCroy.ActiveDSOCtrl");

                if (progType == null)
                {
                    throw new InvalidOperationException("ActiveDSO COM control is not registered on this machine.");
                }

                _scope = Activator.CreateInstance(progType) ??
                         throw new InvalidOperationException("Unable to create ActiveDSO COM instance.");

                try
                {
                    _scope.MakeConnection(address);
                    _connected = true;
                }
                catch (COMException ex)
                {
                    _scope = null;
                    throw new InvalidOperationException($"Failed to connect to scope at {address}: {ex.Message}", ex);
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> SendCommandAsync(string command, CancellationToken cancellationToken = default)
        {
            if (!_connected)
            {
                throw new InvalidOperationException("Connect to the scope before sending commands.");
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Command is required.", nameof(command));
            }

            if (_dryRun)
            {
                await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                return $"OK (simulated) :: {command}";
            }

            return await Task.Run(() =>
            {
                _scope.WriteString(command);
                try
                {
                    var response = _scope.ReadString(2000);
                    return (string?)response ?? string.Empty;
                }
                catch
                {
                    var response = _scope.ReadString();
                    return (string?)response ?? string.Empty;
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task LoadSetupAsync(string setupPath, CancellationToken cancellationToken = default)
        {
            if (!_connected)
            {
                throw new InvalidOperationException("Connect to the scope before loading a setup.");
            }

            if (string.IsNullOrWhiteSpace(setupPath))
            {
                throw new ArgumentException("Setup path is required.", nameof(setupPath));
            }

            if (_dryRun)
            {
                await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                return;
            }

            await Task.Run(() =>
            {
                _scope.WriteString($"""app.SaveRecall.Setup.PanelFilename = "{setupPath}"""");
                try
                {
                    _scope.WriteString("app.SaveRecall.Setup.DoRecallPanel", true);
                }
                catch
                {
                    _scope.WriteString("app.SaveRecall.Setup.DoRecallPanel");
                }

                // After loading a setup, measurement configuration may be overwritten; reset flag.
                _measurementsConfigured = false;
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<MeasurementResult>> CaptureAndReadMeasurementsAsync(CancellationToken cancellationToken = default)
        {
            if (!_connected)
            {
                throw new InvalidOperationException("Connect to the scope before capturing measurements.");
            }

            if (_dryRun)
            {
                await Task.Delay(150, cancellationToken).ConfigureAwait(false);
                var now = DateTime.Now;
                var results = new List<MeasurementResult>();
                var rnd = new Random();
                for (var ch = 1; ch <= 4; ch++)
                {
                    results.Add(new MeasurementResult { Channel = $"C{ch}", Name = "Amplitude", Unit = "V", Value = Math.Round(0.5 + rnd.NextDouble(), 3), Timestamp = now });
                    results.Add(new MeasurementResult { Channel = $"C{ch}", Name = "Frequency", Unit = "Hz", Value = Math.Round(1_000_000 + rnd.NextDouble() * 1000, 3), Timestamp = now });
                    results.Add(new MeasurementResult { Channel = $"C{ch}", Name = "Rise Time", Unit = "s", Value = Math.Round(1e-8 + rnd.NextDouble() * 1e-7, 10), Timestamp = now });
                }
                return results;
            }

            return await Task.Run(() =>
            {
                ConfigureMeasurements();
                TriggerSingleAcquisition();
                WaitForAcquisitionComplete();
                return ReadMeasurements();
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task DisconnectAsync()
        {
            if (!_connected)
            {
                return;
            }

            if (_dryRun)
            {
                _connected = false;
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    _scope?.CloseConnection();
                }
                catch
                {
                    // Best-effort cleanup only.
                }
                finally
                {
                    _scope = null;
                    _connected = false;
                }
            }).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        private void ConfigureMeasurements()
        {
            if (_measurementsConfigured)
            {
                return;
            }

            // Configure P1/P2/P3 per channel to AMPL/FREQ/RISE respectively.
            for (var ch = 1; ch <= 4; ch++)
            {
                _scope.WriteString($"""app.Measure.C{ch}.P1.ParamEngine = "AMPL"""" );
                _scope.WriteString($"""app.Measure.C{ch}.P2.ParamEngine = "FREQ"""" );
                _scope.WriteString($"""app.Measure.C{ch}.P3.ParamEngine = "RISE"""" );
            }

            _measurementsConfigured = true;
        }

        private void TriggerSingleAcquisition()
        {
            _scope.WriteString("app.Acquisition.Single");
        }

        private void WaitForAcquisitionComplete()
        {
            // Poll a few times; if status is unknown, fall back to a short delay.
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    _scope.WriteString("app.Acquisition.Status");
                    var statusObj = _scope.ReadString(500);
                    var status = statusObj?.ToString() ?? string.Empty;
                    if (IsAcquisitionIdle(status))
                    {
                        return;
                    }
                }
                catch
                {
                    // Ignore and try again.
                }

                Thread.Sleep(150);
            }

            Thread.Sleep(200);
        }

        private static bool IsAcquisitionIdle(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            status = status.Trim().ToUpperInvariant();
            return status.Contains("IDLE", StringComparison.OrdinalIgnoreCase) ||
                   status.Contains("STOP", StringComparison.OrdinalIgnoreCase) ||
                   status.Contains("READY", StringComparison.OrdinalIgnoreCase) ||
                   status == "0" || status == "1";
        }

        private IReadOnlyList<MeasurementResult> ReadMeasurements()
        {
            var results = new List<MeasurementResult>();
            var now = DateTime.Now;

            for (var ch = 1; ch <= 4; ch++)
            {
                results.Add(new MeasurementResult
                {
                    Channel = $"C{ch}",
                    Name = "Amplitude",
                    Unit = "V",
                    Value = ReadDouble($"""app.Measure.C{ch}.P1.Out.Result.Value""" ),
                    Timestamp = now
                });
                results.Add(new MeasurementResult
                {
                    Channel = $"C{ch}",
                    Name = "Frequency",
                    Unit = "Hz",
                    Value = ReadDouble($"""app.Measure.C{ch}.P2.Out.Result.Value""" ),
                    Timestamp = now
                });
                results.Add(new MeasurementResult
                {
                    Channel = $"C{ch}",
                    Name = "Rise Time",
                    Unit = "s",
                    Value = ReadDouble($"""app.Measure.C{ch}.P3.Out.Result.Value""" ),
                    Timestamp = now
                });
            }

            return results;
        }

        private double ReadDouble(string command)
        {
            try
            {
                _scope.WriteString(command);
                var response = (string?)_scope.ReadString(1000) ?? string.Empty;
                if (double.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
            catch
            {
                // Ignore and return NaN below.
            }

            return double.NaN;
        }
    }
}
