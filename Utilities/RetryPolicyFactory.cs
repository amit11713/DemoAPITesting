using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RestSharp;
using System.Net;

namespace DemoAPITesting.Utilities;

public static class RetryPolicyFactory
{
    public static AsyncRetryPolicy<RestResponse> CreateRetryPolicy(int maxRetries, int delayInMs, ILogger logger)
    {
        return Policy<RestResponse>
            .Handle<Exception>()
            .OrResult(response =>
            {
                return response.StatusCode == HttpStatusCode.TooManyRequests ||
                       response.StatusCode == HttpStatusCode.InternalServerError ||
                       response.StatusCode == HttpStatusCode.BadGateway ||
                       response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                       response.StatusCode == HttpStatusCode.GatewayTimeout ||
                       response.StatusCode == 0;
            })
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromMilliseconds(delayInMs * Math.Pow(2, retryAttempt - 1)),
                (result, timeSpan, retryCount, context) =>
                {
                    if (result.Exception != null)
                    {
                        logger.LogWarning(
                            "{Class}.{Method}: Request failed with exception {ExceptionType}: {ExceptionMessage}. Retrying after {RetryTimeSpan}ms (Attempt {RetryCount} of {MaxRetries})",
                            nameof(RetryPolicyFactory), "CreateRetryPolicy",
                            result.Exception.GetType().Name,
                            result.Exception.Message,
                            timeSpan.TotalMilliseconds,
                            retryCount,
                            maxRetries);
                    }
                    else
                    {
                        logger.LogWarning(
                            "{Class}.{Method}: Request failed with status code {StatusCode}. Retrying after {RetryTimeSpan}ms (Attempt {RetryCount} of {MaxRetries})",
                            nameof(RetryPolicyFactory), "CreateRetryPolicy",
                            result.Result.StatusCode,
                            timeSpan.TotalMilliseconds,
                            retryCount,
                            maxRetries);
                    }
                });
    }
}
