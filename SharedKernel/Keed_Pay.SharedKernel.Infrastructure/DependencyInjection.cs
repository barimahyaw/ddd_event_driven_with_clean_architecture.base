using Hangfire;
using Hangfire.Redis.StackExchange;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using Keed_Digital.SharedKernel.Infrastructure.BackgroundJobs;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure.Cache;
using Keed_Digital.SharedKernel.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Security.Claims;
using System.Text;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Data;


namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelExternalServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>()
            .TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        return services;
    }

    public static IServiceCollection AddMemoryCacheService(this IServiceCollection services)
        => services.AddSingleton<ICacheManager, MemoryCacheManager>();

    public static IServiceCollection AddDistributedCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheConnection = Environment.GetEnvironmentVariable("CACHE_CONNECTION_STRING")
            ?? configuration["CACHE_CONNECTION_STRING"]
            ?? throw new ArgumentNullException("CACHE_CONNECTION_STRING");

        services.AddScoped(cfg =>
        {
            var multiplexer = ConnectionMultiplexer.Connect(cacheConnection);
            return multiplexer.GetDatabase();
        });

        services.AddScoped<ICacheManager, MemoryCacheManager>();

        return services;
    }

    public static IServiceCollection AddJwtAuthenticationConfiguration(this IServiceCollection services)
    {
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_ISSUER_SIGNING_KEY")!)
            ?? throw new ArgumentNullException("JWT_ISSUER_SIGNING_KEY");

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(bearer =>
        {
            bearer.RequireHttpsMetadata = false;
            bearer.SaveToken = true;
            bearer.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            bearer.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = c =>
                {
                    if (c.Exception is SecurityTokenExpiredException)
                    {
                        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail(new Error("expired_token", "The Token is expired.")));
                        return c.Response.WriteAsync(result);
                    }
                    else
                    {
                        #if DEBUG
                            c.NoResult();
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        #else
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("An unhandled error has occurred."));
                            return c.Response.WriteAsync(result);
                        #endif
                    }
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail(new Error("unauthorized", "You are not Authorized.")));
                        return context.Response.WriteAsync(result);
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(Result.Fail(new Error("unauthorized_access", "You are not authorized to access this resource.")));
                    return context.Response.WriteAsync(result);
                },
            };
        });
        services.AddHttpContextAccessor();
        services.AddDataProtection();
        return services;
    }

    public static IServiceCollection AddHangfireConfiguration<T>(this IServiceCollection services, IConfiguration configuration, string prefix = default!)
        where T : DbContext
    {
        var options = new RedisStorageOptions
        {
            SucceededListSize = 10000,
            DeletedListSize = 1000,
            InvisibilityTimeout = TimeSpan.FromSeconds(30),
            ExpiryCheckInterval = TimeSpan.FromSeconds(30),
        };

        if (!string.IsNullOrWhiteSpace(prefix)) options.Prefix = prefix;       
       
        var connectionString = Environment.GetEnvironmentVariable("CACHE_CONNECTION_STRING")
            ?? configuration["CACHE_CONNECTION_STRING"]
            ?? throw new ArgumentNullException("CACHE_CONNECTION_STRING");

        services.AddHangfire(config =>
        {
            config.UseRedisStorage(connectionString, options);
            config.UseSerilogLogProvider();
        });

        services.AddHangfireServer();

        services.AddScoped<IOutBoxMessagesProcessingJob, OutBoxMessagesProcessingJob<T>>();

        return services;
    }

    public static WebApplication UseHangfireDashboard(this WebApplication app, string dashboardTitle, string serviceName)
    {
        app.UseHangfireDashboard($"/{serviceName}/workers", new DashboardOptions
        {
            DashboardTitle = dashboardTitle,
            Authorization = new[] { new HanfireAuthorizationFilter() },
            AppPath = "/",
            DisplayStorageConnectionString = false,
            IsReadOnlyFunc = context => false,
            IgnoreAntiforgeryToken = true,
        });
        return app;
    }
}
