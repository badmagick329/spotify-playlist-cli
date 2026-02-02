using System.Text;
using SpotifyCli.Infrastructure;
using SpotifyCli.Presentation;

namespace SpotifyCli;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        var userInputHandler = new UserInputHandler();
        var userNotifier = new UserNotifier();
        var userOutput = new UserOutput();
        var app = new App(userInputHandler, userNotifier, userOutput);
        await app.Initialize();
        await app.Menu();
    }
}
