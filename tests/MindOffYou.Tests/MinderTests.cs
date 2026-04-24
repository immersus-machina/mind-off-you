using Xunit;

namespace MindOffYou.Tests;

public class MinderTests
{
    [Fact]
    public async Task Reach_WellAndHandlerSucceeds_AnsweredAndNoMemoryWrites()
    {
        // Arrange
        var (minder, consumer, memory, _, _, _) = Setup();
        memory.State = new Well();

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        var answered = Assert.IsType<Answered<TestResponse>>(reply);
        Assert.Equal("ok", answered.Message.Value);
        Assert.Empty(memory.HeardCalls);
        Assert.Empty(memory.UnheardCalls);
    }

    [Fact]
    public async Task Reach_WellAndHandlerFails_UnansweredZeroAndNotesUnheardAndHookCalled()
    {
        // Arrange
        var (minder, consumer, memory, _, _, _) = Setup();
        memory.State = new Well();
        var cause = new InvalidOperationException("boom");
        var request = new TestRequest();
        consumer.Handler = (_, _) => throw cause;

        // Act
        var reply = await minder.Reach(consumer, request, CancellationToken.None);

        // Assert
        var unanswered = Assert.IsType<Unanswered<TestResponse>>(reply);
        Assert.Equal(TimeSpan.Zero, unanswered.Wait);
        Assert.Single(memory.UnheardCalls);
        Assert.Empty(memory.HeardCalls);
        var (hookInput, hookCause) = Assert.Single(consumer.WhenUnheardCalls);
        Assert.Same(request, hookInput);
        Assert.Same(cause, hookCause);
    }

    [Fact]
    public async Task Reach_WaveringAndHandlerSucceeds_AnsweredAndNotesHeard()
    {
        // Arrange
        var (minder, consumer, memory, _, _, _) = Setup();
        memory.State = new Wavering();

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        Assert.IsType<Answered<TestResponse>>(reply);
        Assert.Single(memory.HeardCalls);
        Assert.Empty(memory.UnheardCalls);
    }

    [Fact]
    public async Task Reach_WaveringAndHandlerFails_UnansweredWithCurrentWaitAndNotesUnheard()
    {
        // Arrange
        var (minder, consumer, memory, _, _, waits) = Setup();
        memory.State = new Wavering();
        waits.Wait = TimeSpan.FromSeconds(7);
        consumer.Handler = (_, _) => throw new InvalidOperationException("boom");

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        var unanswered = Assert.IsType<Unanswered<TestResponse>>(reply);
        Assert.Equal(TimeSpan.FromSeconds(7), unanswered.Wait);
        Assert.Single(memory.UnheardCalls);
    }

    [Fact]
    public async Task Reach_OnTheMendAndVolunteerAllowedAndHandlerSucceeds_AnsweredAndNotesHeard()
    {
        // Arrange
        var (minder, consumer, memory, _, returnCoord, _) = Setup();
        memory.State = new OnTheMend();
        returnCoord.ShouldProceed = true;

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        Assert.IsType<Answered<TestResponse>>(reply);
        Assert.Single(returnCoord.VolunteerCalls);
        Assert.Single(memory.HeardCalls);
    }

    [Fact]
    public async Task Reach_OnTheMendAndVolunteerDenied_UnaskedWithCurrentWait()
    {
        // Arrange
        var (minder, consumer, memory, _, returnCoord, waits) = Setup();
        memory.State = new OnTheMend();
        returnCoord.ShouldProceed = false;
        waits.Wait = TimeSpan.FromSeconds(12);

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        var unasked = Assert.IsType<Unasked<TestResponse>>(reply);
        Assert.Equal(TimeSpan.FromSeconds(12), unasked.Wait);
        Assert.Empty(memory.HeardCalls);
        Assert.Empty(memory.UnheardCalls);
    }

    [Fact]
    public async Task Reach_StrugglingAndNotReadyForCheckIn_UnaskedWithCurrentWaitAndNoCoordinatorCalls()
    {
        // Arrange
        var (minder, consumer, memory, checkInCoord, returnCoord, waits) = Setup();
        memory.State = new Struggling(DateTimeOffset.UtcNow.AddMinutes(5));
        waits.Wait = TimeSpan.FromSeconds(30);

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        var unasked = Assert.IsType<Unasked<TestResponse>>(reply);
        Assert.Equal(TimeSpan.FromSeconds(30), unasked.Wait);
        Assert.Empty(checkInCoord.VolunteerCalls);
        Assert.Empty(returnCoord.VolunteerCalls);
        Assert.Empty(memory.HeardCalls);
        Assert.Empty(memory.UnheardCalls);
    }

    [Fact]
    public async Task Reach_StrugglingAndReadyAndCheckInAllowedAndHandlerSucceeds_AnsweredAndNotesHeard()
    {
        // Arrange
        var (minder, consumer, memory, checkInCoord, _, _) = Setup();
        memory.State = new Struggling(DateTimeOffset.UtcNow.AddMinutes(-1));
        checkInCoord.ShouldProceed = true;

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        Assert.IsType<Answered<TestResponse>>(reply);
        Assert.Single(checkInCoord.VolunteerCalls);
        Assert.Single(memory.HeardCalls);
    }

    [Fact]
    public async Task Reach_StrugglingAndReadyAndCheckInDenied_UnaskedWithCurrentWait()
    {
        // Arrange
        var (minder, consumer, memory, checkInCoord, _, waits) = Setup();
        memory.State = new Struggling(DateTimeOffset.UtcNow.AddMinutes(-1));
        checkInCoord.ShouldProceed = false;
        waits.Wait = TimeSpan.FromSeconds(20);

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        var unasked = Assert.IsType<Unasked<TestResponse>>(reply);
        Assert.Equal(TimeSpan.FromSeconds(20), unasked.Wait);
        Assert.Single(checkInCoord.VolunteerCalls);
        Assert.Empty(memory.HeardCalls);
    }

    [Fact]
    public async Task Reach_CheckingIn_UnaskedWithCurrentWait()
    {
        // Arrange
        var (minder, consumer, memory, _, _, waits) = Setup();
        memory.State = new CheckingIn();
        waits.Wait = TimeSpan.FromSeconds(45);

        // Act
        var reply = await minder.Reach(consumer, new TestRequest(), CancellationToken.None);

        // Assert
        var unasked = Assert.IsType<Unasked<TestResponse>>(reply);
        Assert.Equal(TimeSpan.FromSeconds(45), unasked.Wait);
    }

    [Fact]
    public async Task Reach_PreCancelledToken_ThrowsBeforeAnything()
    {
        // Arrange
        var (minder, consumer, memory, _, _, _) = Setup();
        memory.State = new Well();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act + Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => minder.Reach(consumer, new TestRequest(), cts.Token));
        Assert.Equal(0, memory.RecallCalls);
    }

    [Fact]
    public async Task Reach_WellAndHandlerThrowsOperationCanceled_PropagatesAndNotCountedAsUnheard()
    {
        // Arrange
        var (minder, consumer, memory, _, _, _) = Setup();
        memory.State = new Well();
        consumer.Handler = (_, _) => throw new OperationCanceledException();

        // Act + Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => minder.Reach(consumer, new TestRequest(), CancellationToken.None));
        Assert.Empty(memory.UnheardCalls);
    }

    // --- setup helper + fakes ---

    private static (IMindYou minder, TestNeedCare consumer, FakeRememberCareState memory,
                    FakeCheckInCoordinator checkInCoord, FakeReturnCoordinator returnCoord, FakeWaitMemory waits)
        Setup(Tending? tending = null)
    {
        var builder = new CareBuilder();
        builder.Mind<TestNeedCare>(tending ?? new Tending());
        var registrations = builder.Build();

        var memory = new FakeRememberCareState();
        var checkInCoord = new FakeCheckInCoordinator();
        var returnCoord = new FakeReturnCoordinator();
        var waits = new FakeWaitMemory();

        var minder = new Minder(memory, checkInCoord, returnCoord, waits, registrations);
        var consumer = new TestNeedCare();

        return (minder, consumer, memory, checkInCoord, returnCoord, waits);
    }
}

internal sealed record TestRequest : IRequestCarefully<TestResponse>;

internal sealed record TestResponse(string Value);

internal sealed class TestNeedCare : INeedCare<TestRequest, TestResponse>
{
    public static string CareId => "test";

    public Func<TestRequest, CancellationToken, Task<TestResponse>> Handler { get; set; } =
        (_, _) => Task.FromResult(new TestResponse("ok"));

    public List<(TestRequest Input, Exception Cause)> WhenUnheardCalls { get; } = [];

    public Task<TestResponse> HandleCarefully(TestRequest input, CancellationToken ct)
    {
        return Handler(input, ct);
    }

    public void WhenUnheard(TestRequest input, Exception cause)
    {
        WhenUnheardCalls.Add((input, cause));
    }
}

internal sealed class FakeRememberCareState : IRememberCareState
{
    public CareState State { get; set; } = new Well();
    public int RecallCalls { get; private set; }
    public List<(CareId CareId, DateTimeOffset Now, Tending Tending)> HeardCalls { get; } = [];
    public List<(CareId CareId, DateTimeOffset Now, Tending Tending)> UnheardCalls { get; } = [];

    public Task<CareState> Recall(CareId careId, DateTimeOffset now, Tending tending, CancellationToken ct)
    {
        RecallCalls++;
        return Task.FromResult(State);
    }

    public Task NoteHeard(CareId careId, DateTimeOffset now, Tending tending, CancellationToken ct)
    {
        HeardCalls.Add((careId, now, tending));
        return Task.CompletedTask;
    }

    public Task NoteUnheard(CareId careId, DateTimeOffset now, Tending tending, CancellationToken ct)
    {
        UnheardCalls.Add((careId, now, tending));
        return Task.CompletedTask;
    }
}

internal sealed class FakeCheckInCoordinator : ICoordinateCarefulCheckIns
{
    public bool ShouldProceed { get; set; } = true;
    public List<CareId> VolunteerCalls { get; } = [];

    public Task<IVolunteered> Volunteer(CareId careId, CancellationToken ct)
    {
        VolunteerCalls.Add(careId);
        return Task.FromResult<IVolunteered>(new FakeVolunteered(ShouldProceed));
    }
}

internal sealed class FakeReturnCoordinator : ICoordinateEasefulReturn
{
    public bool ShouldProceed { get; set; } = true;
    public List<CareId> VolunteerCalls { get; } = [];

    public Task<IVolunteered> Volunteer(CareId careId, CancellationToken ct)
    {
        VolunteerCalls.Add(careId);
        return Task.FromResult<IVolunteered>(new FakeVolunteered(ShouldProceed));
    }

    public Task SetCap(CareId careId, int cap, CancellationToken ct) => Task.CompletedTask;
    public Task<int> GetCap(CareId careId, int fallback, CancellationToken ct) => Task.FromResult(fallback);
    public Task RemoveCap(CareId careId, CancellationToken ct) => Task.CompletedTask;
    public Task<long> BumpRamp(CareId careId, CancellationToken ct) => Task.FromResult(0L);
    public Task ResetRamp(CareId careId, CancellationToken ct) => Task.CompletedTask;
}

internal sealed class FakeVolunteered(bool shouldProceed) : IVolunteered
{
    public bool ShouldProceed { get; } = shouldProceed;
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class FakeWaitMemory : IRememberWait
{
    public TimeSpan Wait { get; set; } = TimeSpan.FromSeconds(5);

    public Task<TimeSpan> Recall(CareId careId, TimeSpan fallback, CancellationToken ct) =>
        Task.FromResult(Wait);

    public Task Register(CareId careId, TimeSpan wait, CancellationToken ct) => Task.CompletedTask;
}
