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

    public async Task<Reply<TMessage>> Reach<TRequest, TMessage>(
        INeedCare<TRequest, TMessage> needCare,
        TRequest request,
        CancellationToken ct)
        where TRequest : IRequestCarefully<TMessage>
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
                => await HoldBack<TMessage>(careId, tending, ct),

            _ => throw new UnreachableException(),
        };
    }

    private async Task<Reply<TMessage>> ReachOutCarefully<TRequest, TMessage>(
        INeedCare<TRequest, TMessage> needCare,
        TRequest request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequest : IRequestCarefully<TMessage>
    {
        try
        {
            var response = await needCare.HandleCarefully(request, ct);
            return new Answered<TMessage>(response);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await careMemory.NoteUnheard(careId, now, tending, ct);
            return new Unanswered<TMessage>(TimeSpan.Zero);
        }
    }

    private async Task<Reply<TMessage>> ReachOutWatchfully<TRequest, TMessage>(
        INeedCare<TRequest, TMessage> needCare,
        TRequest request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequest : IRequestCarefully<TMessage>
    {
        try
        {
            var response = await needCare.HandleCarefully(request, ct);
            await careMemory.NoteHeard(careId, now, tending, ct);
            return new Answered<TMessage>(response);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await careMemory.NoteUnheard(careId, now, tending, ct);
            return new Unanswered<TMessage>(await CurrentWait(careId, tending, ct));
        }
    }

    private async Task<Reply<TMessage>> ReachOutEasefully<TRequest, TMessage>(
        INeedCare<TRequest, TMessage> needCare,
        TRequest request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequest : IRequestCarefully<TMessage>
    {
        await using var volunteered = await returnCoordinator.Volunteer(careId, ct);
        if (!volunteered.ShouldProceed)
        {
            return await HoldBack<TMessage>(careId, tending, ct);
        }

        return await ReachOutWatchfully(needCare, request, careId, tending, now, ct);
    }

    private async Task<Reply<TMessage>> ReachOutForPossibleCheckIn<TRequest, TMessage>(
        INeedCare<TRequest, TMessage> needCare,
        TRequest request,
        CareId careId,
        Tending tending,
        DateTimeOffset now,
        CancellationToken ct)
        where TRequest : IRequestCarefully<TMessage>
    {
        await using var volunteered = await checkInCoordinator.Volunteer(careId, ct);
        if (!volunteered.ShouldProceed)
        {
            return await HoldBack<TMessage>(careId, tending, ct);
        }

        return await ReachOutWatchfully(needCare, request, careId, tending, now, ct);
    }

    private async Task<Reply<TMessage>> HoldBack<TMessage>(CareId careId, Tending tending, CancellationToken ct)
    {
        return new Unasked<TMessage>(await CurrentWait(careId, tending, ct));
    }

    private async Task<TimeSpan> CurrentWait(CareId careId, Tending tending, CancellationToken ct)
    {
        return await waitTimeMemory.Recall(careId, tending.HoldAtLeast, ct);
    }
}
