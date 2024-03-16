namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;

public interface ICurrentUserService
{
    long UserId { get; }
    List<KeyValuePair<string, string>> Claims { get; }
    bool IsInRole(string role);
    bool IsInAnyRole(List<string> role);
    string? UserName { get; }
    string? UserRegionId { get; }
    bool IsInZone(string zone);
    List<string> UserZones();
    string Role();
}
