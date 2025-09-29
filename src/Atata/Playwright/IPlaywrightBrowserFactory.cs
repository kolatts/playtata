namespace Atata;

/// <summary>
/// Represents a browser factory interface for creating Playwright browser instances.
/// </summary>
public interface IPlaywrightBrowserFactory
{
    /// <summary>
    /// Gets the browser type (e.g., "chromium", "firefox", "webkit").
    /// </summary>
    string BrowserType { get; }

    /// <summary>
    /// Creates a browser instance asynchronously.
    /// </summary>
    /// <param name="logManager">The log manager.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created browser instance.</returns>
    Task<Microsoft.Playwright.IBrowser> CreateAsync(ILogManager logManager, CancellationToken cancellationToken = default);
}