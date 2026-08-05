using System.Text.Json;

namespace NoatunCrewing.Extensions;

// Default ASP.NET Core TempData only round-trips primitive types, not POCOs,
// so StatusMessage is serialized to a JSON string under one well-known key.
// This replaces the MVC5 base-controller helper that wrote several loose
// TempData string keys ("SuccessMessage", "ErrorMessage", etc).
public static class TempDataExtensions
{
    private const string Key = "StatusMessage";

    public static void SetStatusMessage(this ITempDataDictionary tempData, StatusMessageType type, string text)
    {
        tempData[Key] = JsonSerializer.Serialize(new StatusMessage(type, text));
    }

    public static void SetSuccess(this ITempDataDictionary tempData, string text) =>
        tempData.SetStatusMessage(StatusMessageType.Success, text);

    public static void SetError(this ITempDataDictionary tempData, string text) =>
        tempData.SetStatusMessage(StatusMessageType.Error, text);

    public static void SetWarning(this ITempDataDictionary tempData, string text) =>
        tempData.SetStatusMessage(StatusMessageType.Warning, text);

    public static void SetInfo(this ITempDataDictionary tempData, string text) =>
        tempData.SetStatusMessage(StatusMessageType.Info, text);

    public static StatusMessage? GetStatusMessage(this ITempDataDictionary tempData)
    {
        if (tempData.TryGetValue(Key, out var raw) && raw is string json && !string.IsNullOrWhiteSpace(json))
        {
            return JsonSerializer.Deserialize<StatusMessage>(json);
        }
        return null;
    }
}
