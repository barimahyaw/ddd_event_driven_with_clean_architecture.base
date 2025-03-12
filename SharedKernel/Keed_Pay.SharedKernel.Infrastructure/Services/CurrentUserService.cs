﻿using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _contextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
        var userIdString = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claims => claims.Type == ClaimTypes.NameIdentifier)?.Value;
        if (Ulid.TryParse(userIdString, out var userId))
            UserId = userId;
        Claims = httpContextAccessor.HttpContext?.User.Claims.AsEnumerable()
            .Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList() ?? new List<KeyValuePair<string, string>>();
        UserName = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claims => claims.Type == ClaimTypes.Email)?.Value;
    }

    public Ulid UserId { get; }

    public List<KeyValuePair<string, string>> Claims { get; }

    public bool IsInRole(string role)
    {
        if (_contextAccessor.HttpContext == null) return false;
        return _contextAccessor.HttpContext.User.IsInRole(role);
    }

    public bool IsInAnyRole(List<string> role) => role.Any(role => IsInRole(role));

    public string? UserName { get; }

    public string? UserRegionId
    {
        get => _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claims => claims.Type == "RegionId")?.Value;
    }

    public bool IsInZone(string zone)
    {
        var zoneClaim = Claims.FirstOrDefault(c => c.Key == "Zones");
        var zones = zoneClaim.Value.Split(',').ToList();
        return zones.Any(z => z == zone);
    }

    public List<string> UserZones()
    {
        var zoneClaim = Claims.FirstOrDefault(c => c.Key == "Zones");
        var zones = zoneClaim.Value.Split(',').ToList();
        return zones;
    }

    public string Role() => Claims.FirstOrDefault(c => c.Key == "Role").Value;
}