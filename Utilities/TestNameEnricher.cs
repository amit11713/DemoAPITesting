using Serilog.Core;
using Serilog.Events;
using NUnit.Framework;

namespace DemoAPITesting.Utilities;

/// <summary>
/// Serilog enricher that adds the current NUnit test name to the log context.
/// This allows for creating separate log files for each test execution.
/// </summary>
public class TestNameEnricher : ILogEventEnricher
{
    /// <summary>
    /// Enriches the log event with the current test name from NUnit's TestContext.
    /// </summary>
    /// <param name="logEvent">The log event to enrich</param>
    /// <param name="propertyFactory">Factory for creating log event properties</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var testName = GetCurrentTestName();
        var sanitizedTestName = SanitizeTestName(testName);
        
        var property = propertyFactory.CreateProperty("TestName", sanitizedTestName);
        logEvent.AddPropertyIfAbsent(property);
    }
    
    /// <summary>
    /// Gets the current test name from NUnit's TestContext.
    /// </summary>
    /// <returns>The test name or "Unknown" if not available</returns>
    private static string GetCurrentTestName()
    {
        try
        {
            return TestContext.CurrentContext?.Test?.FullName ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
    
    /// <summary>
    /// Sanitizes the test name to ensure it's valid for use as a file name.
    /// Replaces invalid characters with underscores and limits length.
    /// </summary>
    /// <param name="testName">The original test name</param>
    /// <returns>A sanitized test name suitable for file names</returns>
    private static string SanitizeTestName(string testName)
    {
        if (string.IsNullOrEmpty(testName))
            return "Unknown";
            
        // Replace invalid file name characters with underscores
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = testName;
        
        foreach (var invalidChar in invalidChars)
        {
            sanitized = sanitized.Replace(invalidChar, '_');
        }
        
        // Also replace some additional characters that might cause issues
        sanitized = sanitized.Replace(' ', '_')
                            .Replace(':', '_')
                            .Replace('(', '_')
                            .Replace(')', '_')
                            .Replace('[', '_')
                            .Replace(']', '_')
                            .Replace('<', '_')
                            .Replace('>', '_');
        
        // Limit length to avoid file system limitations
        if (sanitized.Length > 200)
        {
            sanitized = sanitized.Substring(0, 200);
        }
        
        return sanitized;
    }
}