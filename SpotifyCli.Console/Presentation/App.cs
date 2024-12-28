using SpotifyCli.Application;
using SpotifyCli.Core;
using SpotifyCli.Infrastructure;

namespace SpotifyCli.Presentation;

class App
{
    public Client Client { get; private set; }

    public App()
    {
        var config = new AppConfig();
        Client = new Client(config);
    }

    public async Task Initialize() => await Client.Initialize();

    public async Task Run()
    {
        var createFilteredPlaylist = new CreateFilteredPlaylist(Client, "test");
        await createFilteredPlaylist.Initialize();
        Console.WriteLine("Fetching all playlists...");
        await createFilteredPlaylist.SetSourcePlaylists(
            createFilteredPlaylist
                .AllPlaylists.Where(p => p.Name == "2023")
                .Select(p => p.Id)
                .ToList()
        );
        Console.WriteLine("Filtering playlists by release date...");
        await createFilteredPlaylist.FilterByReleaseDate(new ReleaseDate("2023"));
    }
}
