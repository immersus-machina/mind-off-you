namespace MindOffYou;

/// <summary>
/// When several volunteer to reach out, I let a few through — more as they settle back.
/// </summary>
public interface ICoordinateEasefulReturn
{
    /// <summary>
    /// I volunteer to reach out on <paramref name="careId"/>.
    /// </summary>
    /// <remarks>
    /// Several of us may volunteer at once; only some are let through. Whether I'm through is on
    /// <see cref="IVolunteered.ShouldProceed"/> — and I dispose the handle when I'm done.
    /// </remarks>
    Task<IVolunteered> Volunteer(CareId careId, CancellationToken ct);

    /// <summary>
    /// Set the in-flight cap for <paramref name="careId"/>.
    /// </summary>
    Task SetCap(CareId careId, int cap, CancellationToken ct);

    /// <summary>
    /// Read the current cap, or <paramref name="fallback"/> if none is set.
    /// </summary>
    Task<int> GetCap(CareId careId, int fallback, CancellationToken ct);

    /// <summary>
    /// Remove the cap — back to unlimited.
    /// </summary>
    Task RemoveCap(CareId careId, CancellationToken ct);

    /// <summary>
    /// Bump the ramp-progress counter and return the new value.
    /// </summary>
    Task<long> BumpRamp(CareId careId, CancellationToken ct);

    /// <summary>
    /// Reset the ramp-progress counter to zero.
    /// </summary>
    Task ResetRamp(CareId careId, CancellationToken ct);
}
