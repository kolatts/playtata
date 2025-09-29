using Microsoft.Playwright;

namespace Atata;

/// <summary>
/// Represents the navigation functionality between pages and windows using Playwright.
/// </summary>
public sealed class PlaywrightSessionNavigator
{
    private readonly PlaywrightSession _session;

    internal PlaywrightSessionNavigator(PlaywrightSession session) =>
        _session = session;

    /// <summary>
    /// Creates a new page object with navigation.
    /// </summary>
    /// <typeparam name="T">The type of the page object.</typeparam>
    /// <param name="pageObject">
    /// The page object instance. If equals <see langword="null"/>, then creates a new instance using the default constructor.
    /// </param>
    /// <param name="url">
    /// The URL to navigate to.
    /// If <see langword="null"/> and <paramref name="navigate"/> is <see langword="true"/>,
    /// gets the URL from the <see cref="PageObjectNavigationUrlData{TPageObject}"/> of the page object.
    /// </param>
    /// <param name="navigate">
    /// A value indicating whether to navigate to a page URL.
    /// When <see langword="null"/>, uses the default value of <c>true</c>.
    /// </param>
    /// <param name="temporarily">
    /// A value indicating whether to navigate temporarily preserving current page object state.
    /// When <see langword="null"/>, uses the default value of <c>false</c>.
    /// </param>
    /// <returns>The page object.</returns>
    public T To<T>(T? pageObject = null, string? url = null, bool? navigate = null, bool? temporarily = null)
        where T : PageObject<T> =>
        To(pageObject, new GoOptions { Url = url, Navigate = navigate ?? true, Temporarily = temporarily ?? false });

    /// <summary>
    /// Creates a new page object in a new tab.
    /// </summary>
    /// <typeparam name="T">The type of the page object.</typeparam>
    /// <param name="pageObject">
    /// The page object instance. If equals <see langword="null"/>, then creates a new instance using the default constructor.
    /// </param>
    /// <param name="url">
    /// The URL to navigate to.
    /// If <see langword="null"/> and <paramref name="navigate"/> is <see langword="true"/>,
    /// gets the URL from the <see cref="PageObjectNavigationUrlData{TPageObject}"/> of the page object.
    /// </param>
    /// <param name="navigate">
    /// A value indicating whether to navigate to a page URL.
    /// When <see langword="null"/>, uses the default value of <c>true</c>.
    /// </param>
    /// <param name="temporarily">
    /// A value indicating whether to navigate temporarily preserving current page object state.
    /// When <see langword="null"/>, uses the default value of <c>false</c>.
    /// </param>
    /// <returns>The page object.</returns>
    public T ToNewTab<T>(T? pageObject = null, string? url = null, bool? navigate = null, bool? temporarily = null)
        where T : PageObject<T> =>
        To(pageObject, new GoOptions 
        { 
            Url = url, 
            Navigate = navigate ?? true, 
            Temporarily = temporarily ?? false,
            NavigationTarget = "in new page" 
        });

    private T To<T>(T? pageObject, GoOptions options)
        where T : PageObject<T>
    {
        SetContextAsCurrent();

        var currentPageObject = _session.PageObject;

        return currentPageObject is null
            ? GoToInitialPageObject(pageObject, options)
            : GoToFollowingPageObject(currentPageObject, pageObject, options);
    }

    private T GoToInitialPageObject<T>(T? pageObject, GoOptions options)
        where T : PageObject<T>
    {
        pageObject ??= ActivatorEx.CreateInstance<T>();
        pageObject.AssignToSession(_session);
        _session.PageObject = pageObject;

        string? navigationUrl = options.Navigate
            ? pageObject.NavigationUrlData.Value is null or []
                ? _session.BaseUrl
                : pageObject.NavigationUrlData.Value
            : options.Url;

        navigationUrl = PrepareNavigationUrl(
            navigationUrl,
            options,
            pageObject.NavigationUrlData.Variables);

        _session.Log.ExecuteSection(
            new GoToPageObjectLogSection(pageObject, navigationUrl, options.NavigationTarget),
            () =>
            {
                if (navigationUrl?.Length > 0 || options.Navigate)
                {
                    Uri uri = CreateAbsoluteUriForNavigation(navigationUrl);
                    Navigate(uri);
                }

                pageObject.Init();
            });

        pageObject.CompleteInit();

        return pageObject;
    }

    private T GoToFollowingPageObject<T>(
        UIComponent currentPageObject,
        T? nextPageObject,
        GoOptions options)
        where T : PageObject<T>
    {
        nextPageObject ??= ActivatorEx.CreateInstance<T>();

        bool isReturnedFromTemporary = options.Temporarily && currentPageObject == nextPageObject;

        if (!isReturnedFromTemporary)
        {
            nextPageObject.AssignToSession(_session);

            if (!options.Temporarily)
                _session.PageObject = nextPageObject;
        }

        string? navigationUrl = options.Navigate
            ? nextPageObject.NavigationUrlData.Value is null or []
                ? _session.BaseUrl
                : nextPageObject.NavigationUrlData.Value
            : options.Url;

        navigationUrl = PrepareNavigationUrl(
            navigationUrl,
            options,
            nextPageObject.NavigationUrlData.Variables);

        _session.Log.ExecuteSection(
            new GoToPageObjectLogSection(nextPageObject, navigationUrl, options.NavigationTarget),
            () =>
            {
                if (options.NavigationTarget == "in new page")
                    CreateNewPage();

                if (navigationUrl?.Length > 0)
                {
                    Uri uri = CreateAbsoluteUriForNavigation(navigationUrl);
                    Navigate(uri);
                }

                if (!isReturnedFromTemporary)
                {
                    nextPageObject.PreviousPageObject = currentPageObject;
                    nextPageObject.Init();
                }
            });

        if (!isReturnedFromTemporary)
            nextPageObject.CompleteInit();

        return nextPageObject;
    }

    private string? PrepareNavigationUrl(string? navigationUrl, GoOptions options, IEnumerable<KeyValuePair<string, object?>>? navigationUrlVariables)
    {
        if (string.IsNullOrEmpty(navigationUrl))
            return navigationUrl;

        navigationUrl = _session.Variables.FillUriTemplateString(navigationUrl, navigationUrlVariables);

        return !Uri.IsWellFormedUriString(navigationUrl, UriKind.Absolute) && !navigationUrl.StartsWith("/")
            ? "/" + navigationUrl
            : navigationUrl;
    }

    private Uri CreateAbsoluteUriForNavigation(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return new Uri(_session.BaseUrl ?? "about:blank");

        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return new Uri(url);

        string baseUrl = _session.BaseUrl?.TrimEnd('/') ?? "about:blank";
        return new Uri($"{baseUrl}{url}");
    }

    private async void Navigate(Uri uri)
    {
        await _session.Log.ExecuteSectionAsync(
            new LogSection($"Navigate to {uri}", LogLevel.Trace),
            async () =>
            {
                await _session.Page.GotoAsync(uri.ToString()).ConfigureAwait(false);
                _session.IsNavigated = true;
            });
    }

    private async void CreateNewPage()
    {
        await _session.Log.ExecuteSectionAsync(
            new LogSection("Create new page", LogLevel.Trace),
            async () =>
            {
                var page = await _session.Browser.NewPageAsync().ConfigureAwait(false);
                // Note: This is simplified - in a full implementation we'd need to manage multiple pages
                // For now, we'll just replace the current page
                await _session.Page.CloseAsync().ConfigureAwait(false);
                typeof(PlaywrightSession).GetField("_page", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .SetValue(_session, page);
            });
    }

    private void SetContextAsCurrent() =>
        _session.SetAsCurrent();

    private sealed class GoOptions
    {
        public string? Url { get; init; }

        public bool Navigate { get; init; }

        public bool Temporarily { get; init; }

        public string NavigationTarget { get; init; } = "current page";
    }
}