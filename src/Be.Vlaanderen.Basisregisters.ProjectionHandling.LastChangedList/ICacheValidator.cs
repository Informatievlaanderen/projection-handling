namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICacheValidator
    {
        Task<bool> CanCache(long position, CancellationToken ct);
    }
}
