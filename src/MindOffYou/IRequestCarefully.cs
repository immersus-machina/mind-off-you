namespace MindOffYou;

/// <summary>
/// I ask carefully, with an expected response.
/// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
public interface IRequestCarefully<out TMessage>
#pragma warning restore CA1040 // Avoid empty interfaces
{
}
