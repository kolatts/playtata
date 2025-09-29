using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// Represents a screenshot taker using Playwright.
/// </summary>
public class PlaywrightScreenshotTaker : IScreenshotTaker
{
    private readonly PlaywrightSession _session;

    public PlaywrightScreenshotTaker(PlaywrightSession session)
    {
        _session = session;
    }

    public FileSubject? TakeScreenshot(string? title = null)
    {
        return TakeScreenshot(ScreenshotKind.Default, title);
    }

    public FileSubject? TakeScreenshot(ScreenshotKind kind, string? title = null)
    {
        // For now, we'll return null to avoid complex file management
        // In a full implementation, this would save the screenshot to a file
        // and return a FileSubject for it
        return null;
    }
}