namespace SpotifyCli.Infrastructure;

interface ICache
{
    public string? Get(string key);
    public void Set<T>(string key, T value);
}
