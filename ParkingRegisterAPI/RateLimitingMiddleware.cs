using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;

namespace ParkingRegisterAPI
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public RateLimitingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var decorator = endpoint?.Metadata.GetMetadata<LimitRequests>();

            // all endpoints have LimitRequests decorators at this point so no need to check if decorator is null

            var key = $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
            var clientStatistics = await GetClientStatisticsByKey(key);
            if (clientStatistics != null &&
                DateTime.UtcNow < clientStatistics.LastSuccessfulResponseTime.AddSeconds(decorator.TimeWindow) &&
                clientStatistics.NumberOfRequestsCompletedSuccessfully == decorator.MaxRequests)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            UpdateClientStatisticsStorage(key, decorator.TimeWindow);
            await _next(context);
        }

        private async Task<ClientStatistics?> GetClientStatisticsByKey(string key)
        {
            byte[] objectFromCache = await _cache.GetAsync(key.ToString());
            if (objectFromCache != null)
            {
                var jsonToDeserialize = System.Text.Encoding.UTF8.GetString(objectFromCache);
                var cachedResult = JsonSerializer.Deserialize<ClientStatistics>(jsonToDeserialize);
                if (cachedResult != null)
                {
                    return cachedResult;
                }
            }

            return null;
        }

        private async void UpdateClientStatisticsStorage(string key, int timeWindow)
        {
            var clientStatistics = await GetClientStatisticsByKey(key);
            if (clientStatistics == null) clientStatistics = new ClientStatistics();
            clientStatistics.LastSuccessfulResponseTime = DateTime.UtcNow;
            clientStatistics.NumberOfRequestsCompletedSuccessfully += 1;
            byte[] objectToCache = JsonSerializer.SerializeToUtf8Bytes(clientStatistics);
            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(timeWindow));
            await _cache.SetAsync(key, objectToCache, cacheEntryOptions);
        }

        public class ClientStatistics
        {
            public DateTime LastSuccessfulResponseTime { get; set; }
            public int NumberOfRequestsCompletedSuccessfully { get; set; }
        }
    }

    public static class RateLimitingExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
