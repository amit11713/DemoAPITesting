namespace DemoAPITesting.Configurations;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayInMilliseconds { get; set; } = 1000;
}
