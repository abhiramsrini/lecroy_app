using System.Threading;
using System.Threading.Tasks;

namespace LecroyScopeWinForms.Scope
{
    public interface IScopeClient : IAsyncDisposable
    {
        bool IsConnected { get; }
        bool IsDryRun { get; }

        Task ConnectAsync(string address, CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        Task<string> SendCommandAsync(string command, CancellationToken cancellationToken = default);
        Task LoadSetupAsync(string setupPath, CancellationToken cancellationToken = default);
    }
}
