// Support/Hooks.cs
using BrowserStack;
using OpenQA.Selenium;
using Reqnroll;
using System;
using System.Collections.Generic;

[Binding]
public class Hooks
{
    private readonly ScenarioContext _scenarioContext;
    private static Local _browserStackLocal;

    public Hooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static void InitializeBrowserStackLocal()
    {
        var accessKey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY") ?? "LDyExLn7cqdjwheyxECy";
        _browserStackLocal = new Local();

        var localArgs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("key", accessKey),
            new KeyValuePair<string, string>("localIdentifier", "LOCAL_TEST_ID")
        };

        _browserStackLocal.start(localArgs);
    }

    [BeforeScenario]
    public void CreateBrowserStackDriver()
    {
        var capabilities = new Dictionary<string, object>
        {
            { "browserName", "Chrome" },
            { "browserVersion", "latest" },
            { "os", "Windows" },
            { "osVersion", "10" },
            { "projectName", "Reqnroll BrowserStack Demo" },
            { "buildName", "Build 1.0" },
            { "sessionName", _scenarioContext.ScenarioInfo.Title },
            { "browserstack.video", "true" },
            { "browserstack.debug", "true" }
        };

        var driver = BrowserStackHelper.CreateDriver(capabilities);
        _scenarioContext.Set(driver, "WebDriver");
    }

    [AfterScenario]
    public void CloseBrowserStackDriver()
    {
        var driver = _scenarioContext.Get<IWebDriver>("WebDriver");

        try
        {
            var status = _scenarioContext.TestError == null ? "passed" : "failed";
            var reason = _scenarioContext.TestError?.Message ?? "Test completed successfully";

            BrowserStackHelper.MarkTestStatus(driver, status, reason);
        }
        finally
        {
            driver.Quit();
        }
    }

    [AfterTestRun]
    public static void CleanupBrowserStackLocal()
    {
        _browserStackLocal?.stop();
    }
}