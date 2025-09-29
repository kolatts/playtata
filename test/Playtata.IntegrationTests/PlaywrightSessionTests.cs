using Microsoft.Playwright;

namespace Atata.IntegrationTests;

[TestFixture]
public class PlaywrightSessionTests
{
    [Test]
    public void CreatePlaywrightSessionBuilder_ShouldHaveCorrectDefaults()
    {
        // Arrange & Act
        var builder = PlaywrightSession.CreateBuilder()
            .UseChrome();

        // Assert
        builder.Should().NotBeNull();
        builder.BrowserFactories.Should().HaveCount(1);
        builder.BrowserFactoryToUse.Should().BeNull(); // Not set until ConfigureSession is called
        builder.DisposeBrowser.Should().BeTrue();
    }

    [Test]
    public void PlaywrightSessionBuilder_ShouldSupportMultipleBrowsers()
    {
        // Arrange & Act
        var builder = PlaywrightSession.CreateBuilder()
            .UseChrome()
            .UseFirefox()
            .UseSafari();

        // Assert
        builder.BrowserFactories.Should().HaveCount(3);
        builder.BrowserFactories[0].BrowserType.Should().Be("chromium");
        builder.BrowserFactories[1].BrowserType.Should().Be("firefox");
        builder.BrowserFactories[2].BrowserType.Should().Be("webkit");
    }

    [Test]
    public void PlaywrightBrowserFactory_ShouldCreateWithCorrectBrowserType()
    {
        // Arrange
        var chromeFactory = new PlaywrightBrowserFactory("chromium");
        var firefoxFactory = new PlaywrightBrowserFactory("firefox");
        var webkitFactory = new PlaywrightBrowserFactory("webkit");

        // Assert
        chromeFactory.BrowserType.Should().Be("chromium");
        firefoxFactory.BrowserType.Should().Be("firefox");
        webkitFactory.BrowserType.Should().Be("webkit");
    }
}