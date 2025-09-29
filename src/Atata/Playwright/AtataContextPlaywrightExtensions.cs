namespace Atata;

/// <summary>
/// Extension methods for AtataContext to support Playwright sessions.
/// </summary>
public static class AtataContextPlaywrightExtensions
{
    /// <summary>
    /// Starts a Playwright session using the specified session builder.
    /// </summary>
    /// <param name="context">The Atata context.</param>
    /// <param name="sessionBuilder">The Playwright session builder.</param>
    /// <returns>The same Atata context instance.</returns>
    public static AtataContext StartPlaywrightSession(this AtataContext context, PlaywrightSessionBuilder sessionBuilder)
    {
        Guard.ThrowIfNull(context);
        Guard.ThrowIfNull(sessionBuilder);

        // For now, use synchronous BuildAsync().GetAwaiter().GetResult()
        // In a real implementation, this should be properly async
        var session = sessionBuilder.BuildAsync().GetAwaiter().GetResult();
        context.Sessions.Add(session);
        
        return context;
    }

    /// <summary>
    /// Starts a Playwright session with Chrome browser.
    /// </summary>
    /// <param name="context">The Atata context.</param>
    /// <param name="configure">Optional configuration action for the session builder.</param>
    /// <returns>The same Atata context instance.</returns>
    public static AtataContext StartPlaywrightSession(this AtataContext context, Action<PlaywrightSessionBuilder>? configure = null)
    {
        var builder = PlaywrightSession.CreateBuilder().UseChrome();
        configure?.Invoke(builder);
        
        return StartPlaywrightSession(context, builder);
    }
}