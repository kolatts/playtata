# [Playtata](https://github.com/kolatts/playtata)

[![Build status](https://dev.azure.com/kolatts/playtata/_apis/build/status/playtata-ci?branchName=main)](https://dev.azure.com/kolatts/playtata/_build/latest?definitionId=17&branchName=main)

C#/.NET web UI test automation full-featured framework based on Microsoft Playwright.
It uses a fluent page object pattern;
has a built-in logging system;
contains a unique triggers functionality;
has a set of ready-to-use components.
One of the key ideas of the framework is to provide a simple and intuitive syntax for defining and using page objects.
A page object implementation requires as less code as possible.
You can describe a page object class without any methods and only have a set of properties marked with attributes representing page components.

*The package targets .NET Standard 2.0, which supports .NET 5+, .NET Framework 4.6.1+ and .NET Core/Standard 2.0+.*

## Features

- **Playwright**.
  Based on [Microsoft Playwright](https://github.com/microsoft/playwright-dotnet) with modern async/await patterns and reliable browser automation.
- **Page object model**.
  Provides a unique fluent page object pattern, which is easy to implement and maintain.
- **Components**.
  Contains a rich set of ready-to-use components for inputs, tables, lists, etc.
- **Integration**.
  Works on any .NET test engine (e.g. NUnit, xUnit, SpecFlow) as well as on CI systems like Jenkins, GitHub Actions, or TeamCity.
- **Triggers**.
  A bunch of triggers to bind with different events to extend component behavior.
- **Verification**.
  A set of fluent assertion methods and triggers for a component and data verification.
- **Configurable**.
  Defines the default component search strategies as well as additional settings.
- **Reporting/Logging**.
  Built-in customizable logging; screenshots and snapshots capturing functionality.
- **Extensible**.
  Designed for extensibility with custom components and behaviors.

## Usage

### Page object

Simple sign-in page object:

```C#
using Playtata;

namespace SampleApp.UITests
{
    using _ = SignInPage;

    [Url("signin")] // Relative URL of the page.
    [VerifyH1] // Verifies that H1 header text equals "Sign In" upon page object initialization.
    public class SignInPage : Page<_>
    {
        [FindByLabel] // Finds <label> element containing "Email" (<label for="email">Email</label>), then finds text <input> element by "id" that equals label's "for" attribute value.
        public TextInput<_> Email { get; private set; }

        [FindById("password")] // Finds password <input> element by id that equals "password" (<input id="password" type="password">).
        public PasswordInput<_> Password { get; private set; }

        [FindByValue(TermCase.Title)] // Finds button element by value that equals "Sign In" (<input value="Sign In" type="submit">).
        public Button<_> SignIn { get; private set; }
    }
}
```

### Test

Usage in the test method:

```C#
[Test]
public void SignIn()
{
    Go.To<SignInPage>()
        .Email.Set("admin@mail.com")
        .Password.Set("abc123")
        .SignIn.Click();
}
```

### Setup

```C#
[SetUp]
public void SetUp()
{
    PlaytataContext.Configure()
        .UseChrome()
        .UseBaseUrl("https://demo.example.com/")
        .Build();
}
```

## Demo

Sample test:

```C#
[Test]
public async Task Create()
{
    await Login()
        .New()
            .ModalTitle.Should.Be("New User")
            .General.FirstName.SetRandom(out string firstName)
            .General.LastName.SetRandom(out string lastName)
            .General.Email.SetRandom(out string email)
            .General.Office.SetRandom(out Office office)
            .General.Gender.SetRandom(out Gender gender)
            .Save()
        .GetUserRow(email).View()
            .AggregateAssert(x => x
                .Header.Should.Be($"{firstName} {lastName}")
                .Email.Should.Be(email)
                .Office.Should.Be(office)
                .Gender.Should.Be(gender)
                .Birthday.Should.Not.Exist()
                .Notes.Should.Not.Exist());
}
```

## Documentation

Coming soon! This framework is based on the concepts from the Atata framework but redesigned for Microsoft Playwright.

## Feedback

Any feedback, issues and feature requests are welcome.

If you faced an issue please report it to [Playtata Issues](https://github.com/kolatts/playtata/issues).

## Contributing

Check out [Contributing Guidelines](CONTRIBUTING.md) for details.

## SemVer

Playtata Framework follows [Semantic Versioning 2.0](https://semver.org/).
We maintain backward compatibility and follow semantic versioning principles for releases.

## License

Playtata is an open source software, licensed under the Apache License 2.0.
See [LICENSE](LICENSE) for details.
