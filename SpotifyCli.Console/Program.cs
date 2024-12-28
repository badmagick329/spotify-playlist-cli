using SpotifyCli.Presentation;

namespace SpotifyCli;

class Program
{
    public static async Task Main(string[] args)
    {
        var app = new App();
        await app.Initialize();
        await app.Run();
    }
}
