using Spectre.Console;
using SpotifyCli.Core;

namespace SpotifyCli.Infrastructure;

public class UserOutput : IUserOutput
{
    public void Write(string text)
    {
        AnsiConsole.MarkupLine(text);
    }
}
