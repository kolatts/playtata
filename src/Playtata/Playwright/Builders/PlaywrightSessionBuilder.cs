using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// Represents a builder for creating and configuring a <see cref="PlaywrightSession"/>.
/// </summary>
public class PlaywrightSessionBuilder : WebSessionBuilder<PlaywrightSession, PlaywrightSessionBuilder>
{
    public PlaywrightSessionBuilder()
    {
    }

    /// <summary>
    /// Gets the browser factories.
    /// </summary>
    public List<IPlaywrightBrowserFactory> BrowserFactories { get; private set; } = [];

    /// <summary>
    /// Gets the browser factory to use.
    /// </summary>
    public IPlaywrightBrowserFactory? BrowserFactoryToUse { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether to dispose the browser
    /// when <see cref="AtataSession.DisposeAsync()"/> method is invoked.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool DisposeBrowser { get; set; } = true;

    /// <summary>
    /// Adds Chrome browser to the browser factories.
    /// </summary>
    /// <param name="optionsInitializer">The browser launch options initializer.</param>
    /// <returns>The same builder instance.</returns>
    public PlaywrightSessionBuilder UseChrome(Action<BrowserTypeLaunchOptions>? optionsInitializer = null) =>
        UseBrowser("chromium", optionsInitializer);

    /// <summary>
    /// Adds Firefox browser to the browser factories.
    /// </summary>
    /// <param name="optionsInitializer">The browser launch options initializer.</param>
    /// <returns>The same builder instance.</returns>
    public PlaywrightSessionBuilder UseFirefox(Action<BrowserTypeLaunchOptions>? optionsInitializer = null) =>
        UseBrowser("firefox", optionsInitializer);

    /// <summary>
    /// Adds Safari browser to the browser factories.
    /// </summary>
    /// <param name="optionsInitializer">The browser launch options initializer.</param>
    /// <returns>The same builder instance.</returns>
    public PlaywrightSessionBuilder UseSafari(Action<BrowserTypeLaunchOptions>? optionsInitializer = null) =>
        UseBrowser("webkit", optionsInitializer);

    private PlaywrightSessionBuilder UseBrowser(string browserType, Action<BrowserTypeLaunchOptions>? optionsInitializer = null)
    {
        var factory = new PlaywrightBrowserFactory(browserType, optionsInitializer);
        BrowserFactories.Add(factory);
        return this;
    }

    /// <summary>
    /// Uses the specified browser factory.
    /// </summary>
    /// <param name="browserFactory">The browser factory.</param>
    /// <returns>The same builder instance.</returns>
    public PlaywrightSessionBuilder UseBrowserFactory(IPlaywrightBrowserFactory browserFactory)
    {
        Guard.ThrowIfNull(browserFactory);

        BrowserFactories.Add(browserFactory);
        return this;
    }

    protected override IScreenshotTaker CreateScreenshotTaker(PlaywrightSession session) =>
        new PlaywrightScreenshotTaker(session);

    protected override IPageSnapshotTaker CreatePageSnapshotTaker(PlaywrightSession session) =>
        new PlaywrightPageSnapshotTaker(session);

    protected override void ConfigureSession(PlaywrightSession session)
    {
        base.ConfigureSession(session);

        BrowserFactoryToUse = BrowserFactories.LastOrDefault()
            ?? throw new InvalidOperationException("No browser factory is specified.");

        session.BrowserFactory = BrowserFactoryToUse;
        session.Variables.SetInitialValue("browser-type", BrowserFactoryToUse.BrowserType);

        session.DisposeBrowser = DisposeBrowser;
    }

    protected override void OnClone(PlaywrightSessionBuilder copy)
    {
        base.OnClone(copy);

        copy.BrowserFactories = [.. BrowserFactories];
        copy.DisposeBrowser = DisposeBrowser;
    }
}