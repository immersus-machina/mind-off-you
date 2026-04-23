# MindOffYou

Distributed resilience for serverless queue consumers. When someone you depend on is under pressure and needs space — back off, be considerate, and resume normal interactions on their terms.

---

## When things are hard

> You said you needed space.
> I know I should,
> I just can't keep my mind off you.

Sometimes those we depend on are going through something. They're not themselves. Normal interactions become strained, then harmful. What they need isn't more contact — it's room to recover.

But we're all reaching out independently, each of us discovering the trouble in our own time, each of us trying to help in ways that don't help. Nobody's talking to each other. They're drowning in our collective good intentions.

## A different way

MindOffYou is what happens when everyone who cares about the same dependency decides to coordinate. One of us notices they're struggling. Everyone knows. We all give space at the same time — not separately, together.

When enough time has passed, one of us checks in gently. The rest wait to hear how it went. If they're doing better, we slowly return — a few of us at first, the rest only as it becomes clear they're holding up.

No flood. No relapse. Just patience, shared.

```csharp
services.AddMindOffYou(options =>
{
    options.Dependency("payments-api", dep =>
    {
        dep.GiveSpace = TimeSpan.FromSeconds(30);
        dep.CheckIn = BackoffStrategy.Gentle;
        dep.ReturnTo = RecoveryPace.Gradual;
    });
});
```

```csharp
public async Task Run(
    [ServiceBusTrigger("orders")] ServiceBusReceivedMessage message,
    ServiceBusMessageActions actions,
    IConsiderate them)
{
    await them.Reach("payments-api", message, actions,
        async ct =>
        {
            await ProcessOrder(message, ct);
            await actions.CompleteMessageAsync(message, ct);
        });
}
```

## What this takes

A shared place to remember together. Redis, for now. Anywhere everyone can reach.

A way to defer contact without giving up on it entirely.

A willingness to treat our dependencies like they might actually be struggling — not adversaries to retry against, but systems that deserve the same consideration we'd want.

## What this isn't

Not a replacement for in-process care. Polly already does that well — keep using it.

Not a broker. Not an orchestrator. Not a framework. A small layer with a particular view of how consumers should treat the services they depend on.

Not a judgment about why things went wrong. Just a response when they did.

## Prior art

- **Polly** — in-process resilience. MindOffYou extends the concept across serverless instances.
- **MassTransit / NServiceBus** — container-hosted consumers with built-in resilience. MindOffYou provides the missing piece for teams who want to stay serverless.
- **Circuit breaker pattern** (_Release It!_, Michael Nygard) — the classical pattern, extended to distributed coordination.

---

Lyrics by [Immersus Machina](https://www.immersus-machina.com)
