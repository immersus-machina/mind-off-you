namespace MindOffYou;

/// <summary>
/// I pick one when several volunteer.
/// </summary>
public interface ICoordinateCarefulCheckIns
{
    /// <summary>
    /// I volunteer to check in on <paramref name="careId"/>.
    /// </summary>
    /// <remarks>
    /// Several of us may volunteer at once; only one is chosen. Whether I'm the one is on
    /// <see cref="IVolunteered.ShouldProceed"/> — and I dispose the handle when my turn's over.
    /// </remarks>
    Task<IVolunteered> Volunteer(CareId careId, CancellationToken ct);
}
