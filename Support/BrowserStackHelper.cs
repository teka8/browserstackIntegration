using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;

public static class BrowserStackHelper
{
    // Replace these with your actual credentials
    private const string UserName = "tekadefar_Pfe1hX";
    private const string AccessKey = "LDyExLn7cqdjwheyxECy";
    private const string LocalIdentifier = "LOCAL_TEST_ID";

    public static IWebDriver CreateDriver(Dictionary<string, object> capabilities)
    {
        // 1. Create the appropriate driver options
        var options = CreateDriverOptions(capabilities);

        // 2. Add all capabilities including BrowserStack authentication
        var browserstackOptions = new Dictionary<string, object>
        {
            ["browserstack.user"] = UserName,
            ["browserstack.key"] = AccessKey,
            ["browserstack.local"] = "true",
            ["browserstack.localIdentifier"] = LocalIdentifier
        };

        // 3. Handle special capabilities
        HandleSpecialCapabilities(capabilities, options);

        // 4. Merge remaining capabilities
        foreach (var capability in capabilities)
        {
            if (capability.Value != null && !IsReservedCapability(capability.Key))
            {
                options.AddAdditionalOption(capability.Key, capability.Value);
            }
        }

        // 5. Add BrowserStack capabilities (must come last)
        foreach (var option in browserstackOptions)
        {
            options.AddAdditionalOption(option.Key, option.Value);
        }

        // 6. Create and return the driver
        return new RemoteWebDriver(
            new Uri($"https://{UserName}:{AccessKey}@hub-cloud.browserstack.com/wd/hub"),
            options);
    }

    private static DriverOptions CreateDriverOptions(Dictionary<string, object> capabilities)
    {
        string browserName = capabilities.TryGetValue("browserName", out var bn)
            ? bn.ToString().ToLower()
            : "chrome";

        return browserName switch
        {
            "firefox" => new FirefoxOptions(),
            "edge" => new EdgeOptions(),
            "safari" => new SafariOptions(),
            _ => new ChromeOptions() // Default to Chrome
        };
    }

    private static void HandleSpecialCapabilities(Dictionary<string, object> capabilities, DriverOptions options)
    {
        // Handle platformName
        if (capabilities.TryGetValue("platformName", out var platformName))
        {
            options.PlatformName = platformName.ToString();
            capabilities.Remove("platformName");
        }

        // Handle browserVersion
        if (capabilities.TryGetValue("browserVersion", out var browserVersion))
        {
            if (options is ChromeOptions chromeOptions)
                chromeOptions.BrowserVersion = browserVersion.ToString();
            else if (options is FirefoxOptions firefoxOptions)
                firefoxOptions.BrowserVersion = browserVersion.ToString();

            capabilities.Remove("browserVersion");
        }
    }

    private static bool IsReservedCapability(string capabilityName)
    {
        var reserved = new HashSet<string> { "browserName", "platformName", "browserVersion" };
        return reserved.Contains(capabilityName);
    }

    public static void MarkTestStatus(IWebDriver driver, string status, string reason)
    {
        try
        {
            var script = "browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\""
                + status + "\", \"reason\": \"" + reason + "\"}}";
            ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to mark test status: {ex.Message}");
        }
    }
}