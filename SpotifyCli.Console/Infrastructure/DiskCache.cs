using Newtonsoft.Json;

namespace SpotifyCli.Infrastructure;

class DiskCache : ICache
{
    const string FileName = "spotifycli-cache.json";
    private Dictionary<string, string> _data = [];

    public string? Get(string key)
    {
        _data.TryGetValue(key, out string? value);
        return value;
    }

    public void Set<T>(string key, T value)
    {
        _data[key] = JsonConvert.SerializeObject(value);
        Save();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(FileName))
            {
                var fileContent = File.ReadAllText(FileName);
                _data =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent) ?? [];
            }
            else
            {
                _data = [];
            }
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            _data = [];
            File.WriteAllText(FileName, JsonConvert.SerializeObject(_data));
        }
    }

    private void Save()
    {
        File.WriteAllText(FileName, JsonConvert.SerializeObject(_data));
    }

    public override string ToString() => _data?.ToString() ?? "<No Data>";
}
