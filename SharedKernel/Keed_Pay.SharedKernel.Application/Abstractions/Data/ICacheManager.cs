namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Data;

public interface ICacheManager
{
    bool Cache<T>(string key, T t, int seconds);
    T GetCache<T>(string key);
    (bool exist, bool success) Remove(string key);
}
