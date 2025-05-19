// Middleware/AuditLogMiddleware.cs
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EventEae1._2_Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EventEae1._2_Backend.Middleware
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;

        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Don't log for static files or swagger
                if (!context.Request.Path.StartsWithSegments("/swagger") &&
                    !context.Request.Path.StartsWithSegments("/uploads"))
                {
                    var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                    var userEmail = context.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value ?? "anonymous";

                    await auditLogService.LogAsync(
                        userId,
                        userEmail,
                        $"{context.Request.Method}_{context.Request.Path}",
                        "Request",
                        null,
                        null,
                        new
                        {
                            StatusCode = context.Response.StatusCode,
                            DurationMs = stopwatch.ElapsedMilliseconds,
                            Path = context.Request.Path,
                            Method = context.Request.Method,
                            CorrelationId = correlationId
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var userEmail = context.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value ?? "anonymous";

                await auditLogService.LogAsync(
                    userId,
                    userEmail,
                    $"{context.Request.Method}_{context.Request.Path}_Error",
                    "Request",
                    null,
                    null,
                    new
                    {
                        Error = ex.Message,
                        StackTrace = ex.StackTrace,
                        DurationMs = stopwatch.ElapsedMilliseconds,
                        Path = context.Request.Path,
                        Method = context.Request.Method,
                        CorrelationId = correlationId
                    }
                );

                throw; // Re-throw the exception after logging
            }
        }
    }
}