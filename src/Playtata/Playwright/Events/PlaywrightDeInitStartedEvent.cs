using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// Represents an event that occurs when Playwright browser and page deinitialization is started.
/// </summary>
public class PlaywrightDeInitStartedEvent
{
    public PlaywrightDeInitStartedEvent(IBrowser browser, IPage page)
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