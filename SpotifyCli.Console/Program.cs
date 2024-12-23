using SpotifyCli.Infrastructure;
using SpotifyCli.TestingStuff;

namespace SpotifyCli;

class Program
{
    public static async Task Main(string[] args)
    {
        var config = new AppConfig();
        await Examples.PlaylistPlayExample(config);
    }
}
