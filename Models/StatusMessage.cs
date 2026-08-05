namespace NoatunCrewing.Models;

public sealed class StatusMessage
{
    public StatusMessageType Type { get; set; }
    public string Text { get; set; } = string.Empty;

    public StatusMessage() { }

    public StatusMessage(StatusMessageType type, string text)
    {
        Type = type;
        Text = text;
    }
}
