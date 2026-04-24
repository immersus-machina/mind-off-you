using System.Diagnostics;

namespace MindOffYou;

internal sealed class Minder(
    IRememberCareState careMemory,
    ICoordinateCarefulCheckIns checkInCoordinator,
    ICoordinateEasefulReturn returnCoordinator,
    IRememberWait waitTimeMemory,
    CareRegistrations registrations,
    TimeProvider? time = null) : IMindYou
{
    private TimeProvider Time => time ?? TimeProvider.System;

    public async Task<Reply<TResponseFormat>> Reach<TRequestFormat, TResponseFormat>(
        INeedCare<TRequestFormat, TResponseFormat> needCare,
        TRequestFormat request,
        CancellationToken ct)
        where TRequestFormat : IRequestCarefully<TResponseFormat>
    {
        ct.ThrowIfCancellationRequested();

        var (careId, tending) = registrations.For(needCare.GetType());
        var now = Time.GetUtcNow();

        var state = await careMemory.Recall(careId, ct);

        return state switch
        {
            Well
                => await ReachOutCarefully(needCare, request, careId, tending, now, ct),

            Wavering
                => await ReachOutWatchfully(needCare, request, careId, tending, now, ct),

            OnTheMend
                => await ReachOutEasefully(needCare, request, careId, tending, now, ct),

            Struggling it when it.IsReadyForCheckIn(now)
                => await ReachOutForPossibleCheckIn(needCare, request, careId, tending, now, ct),

            Struggling or CheckingIn
                => await HoldBack<TResponseFormat>(careId, tending, ct),

            _ => throw new UnreachableException(),
        };
    }

    private async Task<Reply<TResponseFormat>> ReachOutCarefully<TRequestFormat, TResponseFormat>(
        INeedCare<TRequestFormat, TResponseFormat> needCare,
        TRequestFormat request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequestFormat : IRequestCarefully<TResponseFormat>
    {
        try
        {
            var response = await needCare.HandleCarefully(request, ct);
            return new Answered<TResponseFormat>(response);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await careMemory.NoteUnheard(careId, now, tending, ct);
            return new Unanswered<TResponseFormat>(TimeSpan.Zero);
        }
    }

    private async Task<Reply<TResponseFormat>> ReachOutWatchfully<TRequestFormat, TResponseFormat>(
        INeedCare<TRequestFormat, TResponseFormat> needCare,
        TRequestFormat request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequestFormat : IRequestCarefully<TResponseFormat>
    {
        try
        {
            var response = await needCare.HandleCarefully(request, ct);
            await careMemory.NoteHeard(careId, now, tending, ct);
            return new Answered<TResponseFormat>(response);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await careMemory.NoteUnheard(careId, now, tending, ct);
            return new Unanswered<TResponseFormat>(await CurrentWait(careId, tending, ct));
        }
    }

    private async Task<Reply<TResponseFormat>> ReachOutEasefully<TRequestFormat, TResponseFormat>(
        INeedCare<TRequestFormat, TResponseFormat> needCare,
        TRequestFormat request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequestFormat : IRequestCarefully<TResponseFormat>
    {
        await using var volunteered = await returnCoordinator.Volunteer(careId, ct);
        if (!volunteered.ShouldProceed)
        {
            return await HoldBack<TResponseFormat>(careId, tending, ct);
        }

        return await ReachOutWatchfully(needCare, request, careId, tending, now, ct);
    }

    private async Task<Reply<TResponseFormat>> ReachOutForPossibleCheckIn<TRequestFormat, TResponseFormat>(
        INeedCare<TRequestFormat, TResponseFormat> needCare,
        TRequestFormat request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequestFormat : IRequestCarefully<TResponseFormat>
    {
        await using var volunteered = await checkInCoordinator.Volunteer(careId, ct);
        if (!volunteered.ShouldProceed)
        {
            return await HoldBack<TResponseFormat>(careId, tending, ct);
        }

        return await ReachOutWatchfully(needCare, request, careId, tending, now, ct);
    }

    private async Task<Reply<TResponseFormat>> HoldBack<TResponseFormat>(CareId careId, Tending tending, CancellationToken ct)
    {
        return new Unasked<TResponseFormat>(await CurrentWait(careId, tending, ct));
    }

    private async Task<TimeSpan> CurrentWait(CareId careId, Tending tending, CancellationToken ct)
    {
        return await waitTimeMemory.Recall(careId, tending.HoldAtLeast, ct);
    }
}
