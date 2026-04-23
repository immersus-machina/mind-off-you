namespace MindOffYou;

/// <summary>
/// A request to handle carefully, paired with its expected response type.
/// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
public interface IRequestCarefully<out TOut>
#pragma warning restore CA1040 // Avoid empty interfaces
{
}
