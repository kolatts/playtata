using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// Represents an event that occurs when Playwright browser and page initialization is completed.
/// </summary>
public class PlaywrightInitCompletedEvent
{
    public PlaywrightInitCompletedEvent(IBrowser browser, IPage page)
    {
        Browser = browser;
        Page = page;
    }

    /// <summary>
    /// Gets the browser instance.
    /// </summary>
    public IBrowser Browser { get; }

    /// <summary>
    /// Gets the page instance.
    /// </summary>
    public IPage Page { get; }
}