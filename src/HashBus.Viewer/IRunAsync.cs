namespace HashBus.Viewer
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IRunAsync
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
