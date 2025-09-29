using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// <para>
/// Represents a session that manages <see cref="IPage"/> instance
/// and provides a set of functionality to manipulate the page using Microsoft Playwright.
/// </para>
/// <para>
/// The session has additional variable in <see cref="AtataSession.Variables"/>: <c>{browser-type}</c>.
/// </para>
/// </summary>
public class PlaywrightSession : WebSession
{
    private IBrowser _browser = null!;
    private IPage _page = null!;

    public PlaywrightSession() =>
        Go = new(this);

    /// <summary>
    /// Gets the current <see cref="PlaywrightSession"/> instance in scope of <see cref="AtataContext.Current"/>.
    /// Returns <see langword="null"/> if there is no such session or <see cref="AtataContext.Current"/> is <see langword="null"/>.
    /// </summary>
    public static new PlaywrightSession? Current =>
        AtataContext.Current?.Sessions.GetOrNull<PlaywrightSession>();

    /// <inheritdoc cref="WebSession.Report"/>
    public new IWebSessionReport<PlaywrightSession> Report =>
        (IWebSessionReport<PlaywrightSession>)base.Report;

    /// <summary>
    /// Gets the <see cref="PlaywrightSessionNavigator"/> instance,
    /// which provides the navigation functionality between pages and windows.
    /// </summary>
    public PlaywrightSessionNavigator Go { get; }

    internal IPlaywrightBrowserFactory BrowserFactory { get; set; } = null!;

    /// <summary>
    /// Gets the browser instance.
    /// </summary>
    public IBrowser Browser => _browser;

    /// <summary>
    /// Gets the page instance.
    /// </summary>
    public IPage Page => _page;

    /// <summary>
    /// Gets the browser type name.
    /// </summary>
    public string? BrowserType =>
        BrowserFactory?.BrowserType;

    internal bool DisposeBrowser { get; set; }

    /// <summary>
    /// Gets the default control visibility.
    /// The default value is <see cref="Visibility.Any"/>.
    /// </summary>
    public Visibility DefaultControlVisibility { get; internal set; }

    /// <summary>
    /// Gets the UI component access chain scope cache.
    /// </summary>
    public UIComponentAccessChainScopeCache UIComponentAccessChainScopeCache { get; } = new();

    /// <summary>
    /// Creates <see cref="PlaywrightSessionBuilder"/> instance for <see cref="PlaywrightSession"/> configuration.
    /// </summary>
    /// <returns>The created <see cref="PlaywrightSessionBuilder"/> instance.</returns>
    public static PlaywrightSessionBuilder CreateBuilder() => new();

    protected internal override async Task StartAsync(CancellationToken cancellationToken) =>
        await InitBrowserAndPageAsync(cancellationToken)
            .ConfigureAwait(false);

    private async Task InitBrowserAndPageAsync(CancellationToken cancellationToken) =>
        await Log.ExecuteSectionAsync(
            new LogSection("Initialize Browser and Page", LogLevel.Trace),
            async () =>
            {
                _browser = await BrowserFactory.CreateAsync(Log, cancellationToken)
                    .ConfigureAwait(false)
                    ?? throw new PlaywrightInitializationException(
                        $"Browser factory returned null as a browser.");

                var context = await _browser.NewContextAsync().ConfigureAwait(false);
                _page = await context.NewPageAsync().ConfigureAwait(false);

                // TODO: v4. Move these RetrySettings out of here.
                RetrySettings.Timeout = ElementFindTimeout;
                RetrySettings.Interval = ElementFindRetryInterval;

                EventBus.Publish(new PlaywrightInitCompletedEvent(_browser, _page));
            });

    /// <summary>
    /// Restarts the browser and page.
    /// </summary>
    public async Task RestartAsync() =>
        await Log.ExecuteSectionAsync(
            new LogSection("Restart browser"),
            async () =>
            {
                await DisposeCurrentBrowserAndPageAsync().ConfigureAwait(false);
                await InitBrowserAndPageAsync(CancellationToken.None).ConfigureAwait(false);
            });

    protected override async ValueTask DisposeAsyncCore()
    {
        EventBus.Publish(new PlaywrightDeInitStartedEvent(_browser, _page));

        if (DisposeBrowser)
            await DisposeCurrentBrowserAndPageAsync().ConfigureAwait(false);

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async Task DisposeCurrentBrowserAndPageAsync()
    {
        if (_page != null)
        {
            await _page.CloseAsync().ConfigureAwait(false);
            _page = null!;
        }

        if (_browser != null)
        {
            await _browser.CloseAsync().ConfigureAwait(false);
            _browser = null!;
        }
    }
}