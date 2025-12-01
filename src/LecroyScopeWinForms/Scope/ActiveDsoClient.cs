using System;
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
    }
}
