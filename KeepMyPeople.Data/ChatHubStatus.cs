namespace KeepMyPeople.Data;

public static class ChatHubStatus
{
    public static readonly string Connected = "Connected";
    public static readonly string Initializing = "Initializing";
    public static readonly string Initialized = "Initialized";
    public static readonly string SendMessage = "SendMessage";
    public static readonly string SentMessage = "SentMessage";
    public static readonly string ReceivingMessage = "ReceivingMessage";
    public static readonly string ReceivedMessage = "ReceivedMessage";
}

public sealed record SendMessage(string Message);
public sealed record SentMessage(AirUser To);
public sealed record ReceivingMessage(string Message);
public sealed record ReceivedMessage();
public sealed record Initialization;