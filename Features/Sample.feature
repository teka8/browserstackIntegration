Feature: Calculator1

Simple calculator for adding two numbers1


@automation
Scenario: Verify BrowserStack Homepage
    Given I navigate to "https://www.browserstack.com"
    Then the page title should contain "BrowserStack"
