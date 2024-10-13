using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ordering.Application.Interfaces;
using System.Security.Claims;

namespace Ordering.Infrastructure.Data.Interceptors
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;
        private AsyncLocal<string> _currentUserId = new AsyncLocal<string>();

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string? GetCurrentUserId()
        {
            // First, check if we have a user ID set manually
            if (!string.IsNullOrEmpty(_currentUserId.Value))
            {
                return _currentUserId.Value;
            }

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    // Try to get from Headers
                    if (httpContext.Request.Headers.TryGetValue("UserId", out var userId))
                    {
                        return userId.ToString();
                    }

                    // Try to get from Claims if using authentication
                    var userIdClaim = httpContext.User?.FindFirst("sub")?.Value ??
                                     httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        return userIdClaim;
                    }
                }

                _logger.LogWarning("No user ID found in context. Using default value.");
                return "System";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user ID. Using default value.");
                return "System";
            }
        }

        public void SetCurrentUserId(string userId)
        {
            _currentUserId.Value = userId;
        }
    }
}
