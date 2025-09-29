using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// Represents the default Playwright browser factory.
/// </summary>
public class PlaywrightBrowserFactory : IPlaywrightBrowserFactory
{
    private readonly Action<BrowserTypeLaunchOptions>? _optionsInitializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBrowserFactory"/> class.
    /// </summary>
    /// <param name="browserType">The browser type (e.g., "chromium", "firefox", "webkit").</param>
    /// <param name="optionsInitializer">The browser launch options initializer.</param>
    public PlaywrightBrowserFactory(string browserType, Action<BrowserTypeLaunchOptions>? optionsInitializer = null)
    {
        BrowserType = browserType ?? throw new ArgumentNullException(nameof(browserType));
        _optionsInitializer = optionsInitializer;
    }

    /// <inheritdoc/>
    public string BrowserType { get; }

    /// <inheritdoc/>
    public async Task<IBrowser> CreateAsync(ILogManager logManager, CancellationToken cancellationToken = default)
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync().ConfigureAwait(false);

        var browserType = BrowserType.ToLowerInvariant() switch
        {
            "chromium" => playwright.Chromium,
            "firefox" => playwright.Firefox,
            "webkit" => playwright.Webkit,
            _ => throw new NotSupportedException($"Browser type '{BrowserType}' is not supported.")
        };

        var options = new BrowserTypeLaunchOptions();
        _optionsInitializer?.Invoke(options);

        return await browserType.LaunchAsync(options).ConfigureAwait(false);
    }
}